import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
  AfterViewInit,
  OnInit,
  OnDestroy,
  ElementRef,
  Renderer
} from '@angular/core';

import { FormControl } from '@angular/forms';
import { UUID } from 'angular2-uuid';

import { SearchResult } from './search-result.model';
import {Subscription} from "rxjs/Subscription";

declare var vl: any; // global vlaanderen ui

@Component({
  selector: 'ww-form-group-autocomplete',
  templateUrl: 'form-group-autocomplete.template.html',
  styleUrls: ['form-group-autocomplete.style.css']
})
export class FormGroupAutocomplete implements OnInit, AfterViewInit, OnDestroy {
  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;
  @Input() fetch: (s: string) => Promise<SearchResult[]>;

  @Output() valueChanged = new EventEmitter<any>();

  public noResults: boolean = true;

  private data: SearchResult[];

  private _initialValue: SearchResult;
  public get initialValue(): SearchResult {
    return this._initialValue;
  }

  @Input('initialValue')
  public set initialValue(value: SearchResult) {
    this._initialValue = value;
    this.noResults = value === undefined;

    if (value) {
      this.currentValue = this.initialValue.value;

      if (this.inputElement) {
        let e = new CustomEvent('vlaanderen-select-init', {
          bubbles: true,
          cancelable: true
        });
        this.inputElement.dispatchEvent(e);
      }

      if (this.buttonElement)
        this.buttonElement.innerText = this.initialValue.label;

      if (this.selectElement)
        this.selectElement.value = this.initialValue.value;
    }
  }

  private elementAttribute = 'data-id'; // used in vlaanderen-ui.js, so don't change
  private element: HTMLElement;
  private selectElement: HTMLSelectElement;
  private inputElement: HTMLInputElement;
  private buttonElement: HTMLButtonElement;

  private generatedGuid: string;
  private currentValue: string = '';

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    element: ElementRef,
    private renderer: Renderer,
    private changeDetect: ChangeDetectorRef
  ) {
    this.element = element.nativeElement;
  }

  ngOnInit() {
    let nodeList = <HTMLSelectElement[]><any>this.element.querySelectorAll('[data-dynselect]');
    this.selectElement = nodeList[0];

    this.generatedGuid = UUID.UUID();
    this.selectElement.setAttribute(this.elementAttribute, this.generatedGuid);
    this.noResults = !this.hasInitialValue();

    // clear the elements if the value changes to null programmatically
    this.subscriptions.push(this.control.valueChanges.subscribe(value => {
      if (value === null) { // we only want to check for null!
        this.buttonElement.innerText = '';
        this.inputElement.value = '';
        this.selectElement.value = '';
        this.noResults = true;

        this.changeDetect.detectChanges();
      }
    }));
  }

  ngOnDestroy() {
    this.changeDetect.detach();
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  ngAfterViewInit() {
    // input data must be added to the DOM before we "dress" the autocomplete
    vl.select.dress(this.selectElement, this.fetchData.bind(this));

    this.inputElement = <HTMLInputElement>this.element.querySelector('input.input-field');
    this.buttonElement = <HTMLButtonElement>this.element.querySelector('button.js-select__input');

    if (this.hasInitialValue()) {
      this.currentValue = this.initialValue.value;
      this.selectElement.value = this.initialValue.value;
      this.buttonElement.innerText = this.initialValue.label;
    } else {
      this.buttonElement.innerText = this.placeholder;
    }

    this.control.registerOnDisabledChange(disabled => this.setDisabled(disabled));
  }

  onValueChanged(e) {
    if (!(e.srcElement || e.target))
      return;

    let newValue = e.target.value;

    if (newValue === this.currentValue)
      return;

    this.currentValue = newValue;
    this.control.setValue(newValue);
    this.inputElement.value = '';
    this.valueChanged.next(this.buttonElement.innerText);

    if (!this.currentValue) {
      this.noResults = true;
    }
  }

  private hasInitialValue(): boolean {
    if (this.initialValue === undefined || this.initialValue === null)
      return false;

    if (this.initialValue && this.initialValue.value && this.initialValue.label) {
      return true;
    } else {
      return false;
    }
  }

  private fetchData(autocomplete, search): Promise<SearchResult[]> {
    let emptyOption = [{ type: 'option', label: this.placeholder, value: '' }];

    if (!search)
      return new Promise(function (resolve, reject) {
        if (this.currentValue) {
          this.noResults = false;
        } else {
          this.noResults = true;
        }

        this.changeDetect.detectChanges();
        resolve(emptyOption);
      }.bind(this));

    return this.fetch(search).then(function (data) {
      let results = data || [];

      if (this.currentValue) {
        // it should be possible to reset to the emptyOption
        this.noResults = false;
      } else {
        // if the emptyOption is chosen, it depends if we have results
        this.noResults = results.length === 0;
      }
      this.changeDetect.detectChanges();

      return emptyOption.concat(results);
    }.bind(this));
  }

  private setDisabled(value: boolean) {
    if (value) {
      // this.control.disable();
      if (this.buttonElement)
        vl.select.setDisabledState(this.selectElement, 'disabled');

    } else {
      // this.control.enable();
      if (this.buttonElement) {
        vl.select.setDisabledState(this.selectElement, '');

        if (this.focus)
          this.renderer.invokeElementMethod(this.buttonElement, 'focus');
      }
    }
  }
}
