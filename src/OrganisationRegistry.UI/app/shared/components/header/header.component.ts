import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { required } from 'core/validation';

@Component({
  selector: 'ww-header',
  styleUrls: ['./header.style.css'],
  templateUrl: 'header.template.html'
})
export class HeaderComponent implements OnInit {
  public searchForm: FormGroup;

  get isFormValid() {
    return this.searchForm.valid;
  }

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.searchForm = formBuilder.group({
      q: ['', [required, Validators.nullValidator]]
    });
  }

  ngOnInit() {
    this.searchForm.setValue({
      q: ''
    });
  }

  search(value) {
    if (!value.q)
      return;

    this.router.navigate(['/search/blank']).then(() => {
      this.router.navigate(['/search/organisations'], { queryParams: { q: value.q } });

      this.searchForm.setValue({
        q: ''
      });
    });
  }
}

