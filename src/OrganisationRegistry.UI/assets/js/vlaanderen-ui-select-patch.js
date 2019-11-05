/**
 * Monkey patch the dress!!!
 *         __
 *   w  c(..)o   (
 *    \__(-)    __)
 *        /\   (
 *       /(_)___)
 *       w /|
 *        | \
 *       m  m
 *
 *       /| |\
 *      ( \./ )
 *       \ : /
 *       ) : (
 *      /  :  \
 *      |__:__|
 *
 * We can't monkey patch only the dress, because it depends on parts outside of the dress function.
 *
 */

// https://developer.mozilla.org/en-US/docs/Web/API/CustomEvent/CustomEvent#Polyfill
(function () {
  if ( typeof window.CustomEvent === "function" ) return false;

  function CustomEvent ( event, params ) {
    params = params || { bubbles: false, cancelable: false, detail: undefined };
    var evt = document.createEvent( 'CustomEvent' );
    evt.initCustomEvent( event, params.bubbles, params.cancelable, params.detail );
    return evt;
   }

  CustomEvent.prototype = window.Event.prototype;
  window.CustomEvent = CustomEvent;
})();

/**
 * Progressively enhance a select field
 */

// create global vl select function
vl.select = {};
vl.select.dress;

(function () {

  var selectFields                      = document.querySelectorAll('[data-select]'),
      selectContentListItemActiveState  = 'select__cta--active',
      selectContentListItemFocusState   = 'select__cta--focus',
      selectContentListItemHiddenState  = 'select__cta--hidden',
      lastSelectId, lastContainer;

  vl.select.dress = function(selectField, jsonCallbackFunction) {

    /*
    * Variables needed in Generate selects basted on original <select> elements
    */
    var arr                       = generateSelect(selectField),
        arrOptions                = arr[0],
        selectId                  = arr[1],
        selectContainer           = arr[2],
        selectList                = arr[3],
        originalSelectOption      = null,
        activeArrOptions          = arrOptions,
        selectDummyInput          = selectContainer.querySelector('.js-select__input'),
        selectContent             = selectContainer.querySelector('[data-content]'),
        selectContentInput        = selectContent.querySelector('[data-input]'),
        selectContentList         = selectContent.querySelector('[data-records]'),
        selectFocusElems          = selectContainer.querySelectorAll('[data-focus]');

    // https://davidwalsh.name/javascript-debounce-function
    // Returns a function, that, as long as it continues to be invoked, will not
    // be triggered. The function will be called after it stops being called for
    // N milliseconds. If `immediate` is passed, trigger the function on the
    // leading edge, instead of the trailing.
    function debounce(func, wait, immediate) {
      var timeout;
      return function() {
        var context = this, args = arguments;
        var later = function() {
          timeout = null;
          if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow) func.apply(context, args);
      };
    };

    /*
    * Events in select element
    */
    (selectContainer ? selectContainer.addEventListener('vlaanderen-select-init', selectContainerInitEventHandler) : null );
    (selectContainer ? selectContainer.addEventListener('keyup', selectContainerKeyUpEventHandler) : null );
    (selectContainer ? selectContainer.addEventListener('keydown', selectContainerKeyDownEventHandler) : null );
    (selectDummyInput ? selectDummyInput.addEventListener('click', selectDummyInputClickEventHandler) : null );

    [].forEach.call(selectFocusElems, function(el){
      el.addEventListener('blur', selectFocusElemBlurHandler);
    });

    var curOption, curOptionIndex, _keyDefaultHandlerDebounced = debounce(_keyDefaultHandler, 300);

    function selectContainerInitEventHandler(e) {
      e.preventDefault();
      _resetOptions();
      _keyDefaultHandlerDebounced();
    }

    /*
    * Eventhandler | selectContainer keyUp
    */
    function selectContainerKeyUpEventHandler(e){
      e.preventDefault();

      // Abort on tab and shift
      if(e.keyCode == 9 || e.keyCode == 16){
        return;
      }

      if(activeArrOptions.length <= 0){
        _resetOptions();
      }else{
        _setCurrentOption();
      }

      switch(e.keyCode){

        // "Backspace key"
        case 8:
          _resetOptions();
          _keyDefaultHandlerDebounced();
        break;

        // "Esc key"
        case 27:
          setSelectState(false, selectContent, selectDummyInput, selectField);
          selectDummyInput.focus();
        break;

        // "Space key"
        case 32:
          if(selectContent.getAttribute('data-show') !== "true"){
            selectDummyInput.click();
          }
        break;

        // "Enter key"
        case 13:
          if (selectDummyInput.getAttribute('aria-expanded') !== "false") {
            if (curOption)
              curOption.click();

            selectDummyInput.focus();
          }
        break;

        // "Arrow up key"
        case 38:
            _keyUpHandler();
        break;

        // "Arrow down key"
        case 40:
            _keyDownHandler();
        break;

        default:
          if(selectDummyInput.getAttribute('aria-expanded') !== "false"){
            _keyDefaultHandlerDebounced();
          }
        break;
      }
    }

    function _keyUpHandler(e){
      if(selectContent.getAttribute('data-show') !== "true"){
        // Tonen bij arrow down en index één verhogen zodat je op dezelfde positie zit bij het openen
        setSelectState(true, selectContent, selectDummyInput, selectField);
        curOptionIndex++;
      }

      if(curOptionIndex > 0){
        curOptionIndex--;
        curOption.removeAttribute('data-selected');
        var el = selectContentList.querySelector('[data-record][data-index="'+curOptionIndex+'"]');
        el.setAttribute('data-selected','true');
        el.focus();
        if(selectContentInput !== null){
          selectContentInput.focus();
        }else{
          selectDummyInput.focus();
        }
      }
    }

    function _keyDownHandler(){
      if(selectContent.getAttribute('data-show') !== "true"){
        // Tonen bij arrow down en index één minderen zodat je op dezelfde positie zit bij het openen
        setSelectState(true, selectContent, selectDummyInput, selectField);
        curOptionIndex--;
      }

      if(curOptionIndex < (activeArrOptions.length - 1)){
        curOptionIndex++;
        curOption.removeAttribute('data-selected');
        var el = selectContentList.querySelector('[data-record][data-index="'+curOptionIndex+'"]');
        el.setAttribute('data-selected','true');
        el.focus();
        if(selectContentInput !== null){
          selectContentInput.focus();
        }else{
          selectDummyInput.focus();
        }
      }
    }

    function _keyDefaultHandler() {
      if(selectContentInput !== null){
        // Clear activeArrOptions
        var val = selectContentInput.value, first; activeArrOptions = [];

        // If dynamic load is enabled
        if (jsonCallbackFunction !== null && jsonCallbackFunction !== undefined) {
          jsonCallbackFunction(selectField, val).then(function (jsonData) {
            _repopuplateSelect(jsonData);
          });
          return;
        }

        // If dynamic load isn't enabled
        for(var item, i = 0; item = arrOptions[i]; i++) {
          var el = selectContentList.querySelector('[data-record][data-label="'+item+'"]');

          // Set visibility hidden of all items & Remove old index of all items & Remove old focus
          el.setAttribute('data-show', 'false');
          el.removeAttribute('data-index');
          el.removeAttribute('data-selected');

          // If substring is present in string show item and push to array
          if(item.toLowerCase().indexOf(val.toLowerCase() ) > -1){
            el.setAttribute('data-show', 'true');
            activeArrOptions.push(item);
          }
        }

        if(activeArrOptions.length > 0){
          setNoResultsFoundElement("hide", selectField, selectContentList);
          for(var opt, i = 0; opt = activeArrOptions[i]; i++) {
            selectContentList.querySelector('[data-record][data-label="'+opt+'"]').setAttribute('data-index', i);
          }
          // Set focus on first element
          if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
            selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');
        }else{
          setNoResultsFoundElement("show", selectField, selectContentList);
        }

      }
    }

    function _setCurrentOption(){
      curOption = selectContentList.querySelector('[data-record][data-selected="true"]');
      (curOption == null ? curOption = selectContentList.querySelector('[data-record][data-index="0"]') : null);
      curOptionIndex = curOption.getAttribute('data-index');
    }

    function _resetOptions(){
      for(var item, i = 0; item = arrOptions[i]; i++) {
        var el = selectContentList.querySelector('[data-record][data-label="' + item + '"]');
        if (el) {
          el.removeAttribute('data-selected');
          el.setAttribute('data-show', 'true');
          el.setAttribute('data-index', i);
        }
      }

      if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
        selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');

      activeArrOptions = arrOptions;
    }

    function _repopuplateSelect(jsonData){
      // Clear arrOptions
      arrOptions = [];

      var c;
      for(c = selectField.options.length - 1 ; c >= 0 ; c--){
          selectField.remove(c);
      }

      // Loop through the jsonData string
      [].forEach.call(jsonData, function(el){
        switch(el.type){
          case "option":
            var option = document.createElement("option");
            option.text = el.label;
            option.setAttribute('value', el.value);
            selectField.add(option);
            break;
        }
      });

      // Remove nodes in selectList
      while (selectList.hasChildNodes())
        selectList.removeChild(selectList.lastChild);

      // Generate list items based on options in real select
      var i = 0;
      [].forEach.call(selectField.options, function(opt){
        var value = opt.value;
        var label = opt.innerHTML;

        // If item has "data-placeholder" it's used as a placeholder item
        if(opt.hasAttribute('data-placeholder')){
          selectDummyInput.innerHTML = label;
        }else{
          // SelectOption
          var selectOption = document.createElement('div');
          addClass(selectOption, 'select__item');

          // Titel (button wrapper)
          var selectOptionButton = document.createElement('button');
              addClass(selectOptionButton, 'select__cta');

          var closestOptGroup = opt.closest('optgroup');

          selectOptionButton.setAttribute('type', 'button');
          selectOptionButton.setAttribute('data-index', i);
          selectOptionButton.setAttribute('data-value', opt.value);
          selectOptionButton.setAttribute('data-label', opt.label);
          selectOptionButton.setAttribute('data-record','');
          selectOptionButton.setAttribute('data-focus', '');
          selectOptionButton.setAttribute('role', 'option');
          selectOptionButton.setAttribute('tabindex','-1');

          // selected state for first elem
          if(i == 0){
            selectOptionButton.setAttribute('data-selected', 'true');
            addClass(selectOptionButton, selectContentListItemActiveState);
          }

          // Titel (span wrapper)
          var selectOptionTitleSpan = document.createElement("span");
              addClass(selectOptionTitleSpan, 'select__cta__title');
              selectOptionTitleSpan.innerHTML = label;

          // Appends
          selectOptionButton.appendChild(selectOptionTitleSpan);
          selectOption.appendChild(selectOptionButton);

          selectList.appendChild(selectOption);

          // Add to arrOptions array
          arrOptions.push(label);
          i++;
        }
      });

      activeArrOptions = arrOptions;
      setRecordEvents();
    }

    /*
    * Eventhandler | selectContainer keyDown
    */
    function selectContainerKeyDownEventHandler(e){
      switch(e.keyCode){
        case 13: case 38: case 40:
        e.preventDefault();
        break;
      }
    }

    /*
    * Eventhandler | selectDummyInput Click
    */
    function selectDummyInputClickEventHandler(e){
      if(selectContent.getAttribute('data-show') === "false"){
        // Show select
        setSelectState(true, selectContent, selectDummyInput, selectField);
        // Set focus on search if present
        selectContentInput.focus();
        // Set selected option or first option active
        if(originalSelectOption !== null){
          selectContentList.querySelector('[data-record][data-label="'+originalSelectOption+'"]').setAttribute('data-selected', 'true');
        }else{
          if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
            selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');
        }
      }
      else{
        setSelectState(false, selectContent, selectDummyInput, selectField);
        selectDummyInput.focus();
      }
    }

    /*
    * selectFocusElemBlurHandler();
    * Used to check the focus state and close the select when focus is outside of the element
    */
    function selectFocusElemBlurHandler(e){
      window.setTimeout(function(){
        var parent = document.activeElement.closest('.js-select[data-id="' + selectId + '"]');
        if(parent === null){
          setSelectState(false, selectContent, selectDummyInput, selectField);
        }

      }, 200);
    };

    /*
    * Loop through dynamically generated records
    */
    setRecordEvents();
    function setRecordEvents(){
      var selectContentListItems = selectContent.querySelectorAll('[data-record]');

      [].forEach.call(selectContentListItems, function(item){
        item.addEventListener('click', function(e){
          var lbl = item.getAttribute('data-label');
          var val = item.getAttribute('data-value');

          // Set selected state to original select
          originalSelectOption = setOriginalSelectFieldOption(selectId, val);

          // Set label in dummy input
          selectDummyInput.innerHTML = lbl;

          // Close select
          setSelectState(false, selectContent, selectDummyInput, selectField);
          selectDummyInput.focus();

          // Remove active class of alle elements
          [].forEach.call(selectContentListItems, function(item2){
            removeClass(item2, selectContentListItemActiveState);
            item2.removeAttribute('data-selected');
          });

          // Add active class to selected element
          addClass(item, selectContentListItemActiveState);
        });
      });
    };
  };

  /*
  * setDisabledState()
  * Sets disabled state of both native and generated select
  * @param selectField(object)
  * @param state(boolean)
  */
  vl.select.setDisabledState = function(selectField, state) {
    var selectContainer   = selectField.closest('.js-select');
    var selectDummyInput  = selectContainer.querySelector('.js-select__input');

    if (state) {
      addClass(selectDummyInput, 'input-field--disabled');
      selectField.setAttribute('disabled', state);
      selectDummyInput.setAttribute('disabled', state);
    } else {
      removeClass(selectDummyInput, 'input-field--disabled');
      selectField.removeAttribute('disabled');
      selectDummyInput.removeAttribute('disabled');
    }
  };

  /*
  * Loop through all select fields
  */
  // [].forEach.call(selectFields, function(selectField) {
  //   vl.select.dress(selectField);
  // });

  /*
  * setVisibilityAttributes()
  * Setting the general data attributes & aria tags
  */
  function setVisibilityAttributes(field, dataShow, ariaHidden){
      field.setAttribute('data-show',   dataShow);
      field.setAttribute('aria-hidden', ariaHidden);
  }

  /*
  * setSelectState()
  * Setting the general data attributes & aria tags of the generated select
  */
  function setSelectState(isShown, selectContent, selectDummyInput, selectField){
    if(isShown){
      var dataShow = true,
          ariaHidden = false,
          ariaExpanded = true;
    }else{
      var dataShow = false,
          ariaHidden = true,
          ariaExpanded = false;

          selectField.focus();
          window.setTimeout(function(){
            selectField.blur();
            if(selectField.getAttribute('data-has-error') == "true"){
              addClass(selectDummyInput, 'error');
            }else{
              removeClass(selectDummyInput, 'error');
            }
          }, 1);
    }

    selectContent.setAttribute('data-show', dataShow);
    selectContent.setAttribute('aria-hidden', ariaHidden);
    selectDummyInput.setAttribute('aria-expanded', ariaExpanded);
  }

  /*
  * generateSelect()
  * Generating the ehanced select
  */
  function generateSelect(selectField){
    // Hide normal select field
    addClass(selectField, 'u-visually-hidden');
    selectField.setAttribute('aria-hidden','true');

    // Variables
    var arr = [], uniqId = uniqueId();

    // Set selectContainer
    var selectContainer = selectField.closest('.js-select');

    // Get data-id or generate one
    if(selectField.hasAttribute('data-id')){
      uniqId = selectField.getAttribute('data-id');
      selectContainer.setAttribute('data-id', uniqId);
    }else{
      selectContainer.setAttribute('data-id', uniqId);
    }

    // Fake select field
    var selectDummyInput = document.createElement("button");
        selectDummyInput.setAttribute('type','button');

        // if you uncomment this, you cannot easily tab through anymore, due to the setTimeout on blur
        //selectDummyInput.setAttribute('data-focus', '');
        selectDummyInput.setAttribute('id', uniqId);
        selectDummyInput.setAttribute('aria-haspopup', 'true');
        selectDummyInput.setAttribute('aria-expanded', 'false');
        addClass(selectDummyInput, 'js-select__input');
        if(selectField.hasAttribute('disabled') && selectField.getAttribute('disabled') !== "false")
          selectDummyInput.setAttribute('disabled','true');

        selectContainer.insertBefore(selectDummyInput, selectContainer.firstChild);


    // Select Wrapper
    var selectWrapper = document.createElement("div");
        addClass(selectWrapper, 'select__wrapper');
        selectWrapper.setAttribute('data-content','');
        selectWrapper.setAttribute('aria-labelledby',uniqId);
        setVisibilityAttributes(selectWrapper, false, true);

        selectContainer.appendChild(selectWrapper);

        // Select Form Wrapper

        var selectForm = document.createElement("div");
            addClass(selectForm, 'select__form');

            selectWrapper.appendChild(selectForm);

            // Select Form Input
            var selectFormInput = document.createElement('input');
                selectFormInput.setAttribute('type','text');
                selectFormInput.setAttribute('autocomplete','off');
                addClass(selectFormInput, 'input-field');
                addClass(selectFormInput, 'input-field--block');
                selectFormInput.setAttribute('data-input','');
                selectFormInput.setAttribute('data-focus', '');
                selectFormInput.setAttribute('value','');
                selectFormInput.setAttribute('tabindex','-1');
                selectFormInput.setAttribute('aria-describedby', 'selectFormInputDescription' + uniqId);
                selectFormInput.setAttribute('aria-haspopup', 'listbox"');

                selectForm.appendChild(selectFormInput);

            var selectFormInputDescription = document.createElement('span');
                selectFormInputDescription.setAttribute('id','selectFormInputDescription' + uniqId);
                selectFormInputDescription.innerHTML = "U bevindt zich in de zoekfunctie van een dropdown menu in een formulier. Navigeer door de opties met ctrl + alt + pijltjes en selecteer met enter. Gebruik escape om de dropdown te sluiten.";
                addClass(selectFormInputDescription, 'u-visually-hidden');

                selectForm.appendChild(selectFormInputDescription);


        // Select List Wrapper
        var selectListWappper = document.createElement('div');
                addClass(selectListWappper,'select__list-wrapper');
                selectListWappper.setAttribute('role','listbox');

                selectWrapper.appendChild(selectListWappper);

                // Select List
                var selectList = document.createElement('section');
                    addClass(selectList, 'select__list');
                    selectList.setAttribute('data-records','');

                    selectListWappper.appendChild(selectList);

                    // Generate option groups based on optgroups in real select
                    var optgroups = selectField.querySelectorAll('optgroup');
                    if(optgroups.length > 0){
                      [].forEach.call(optgroups, function(optgroup){
                        var label = optgroup.getAttribute('label');
                        var selectOptionGroupWrapper = document.createElement('section');
                        addClass(selectOptionGroupWrapper, 'select__group');
                        selectOptionGroupWrapper.setAttribute('data-label', label);
                        selectOptionGroupWrapper.setAttribute('role', 'group');
                        selectList.appendChild(selectOptionGroupWrapper);

                        var selectOptionGroupTitle = document.createElement('h1');
                        selectOptionGroupTitle.innerHTML = label;

                        selectOptionGroupWrapper.appendChild(selectOptionGroupTitle);
                      });
                    }

                    // Generate list items based on options in real select
                    var i = 0;
                    [].forEach.call(selectField.options, function(opt){
                      var value = opt.value;
                      var label = opt.innerHTML;

                      // If item has "data-placeholder" it's used as a placeholder item
                      if(opt.hasAttribute('data-placeholder')){
                        selectDummyInput.innerHTML = label;
                      }else{
                        // SelectOption
                        var selectOption = document.createElement('div');
                        addClass(selectOption, 'select__item');

                        // Titel (button wrapper)
                        var selectOptionButton = document.createElement('button');
                            addClass(selectOptionButton, 'select__cta');
                            // If option is selected set the element active and change the label in the DummyInput
                            if(opt.selected){
                              addClass(selectOptionButton, selectContentListItemActiveState);
                              selectDummyInput.innerHTML = label;
                              selectOptionButton.setAttribute('aria-selected', true);
                            }else{
                              selectOptionButton.setAttribute('aria-selected', false);
                            }

                            var closestOptGroup = opt.closest('optgroup');

                            selectOptionButton.setAttribute('type', 'button');
                            selectOptionButton.setAttribute('data-index', i);
                            selectOptionButton.setAttribute('data-value', opt.value);
                            selectOptionButton.setAttribute('data-label', opt.label);
                            selectOptionButton.setAttribute('data-record','');
                            selectOptionButton.setAttribute('data-focus', '');
                            selectOptionButton.setAttribute('role', 'option');
                            selectOptionButton.setAttribute('tabindex','-1');

                            // Titel (span wrapper)
                            var selectOptionTitleSpan = document.createElement("span");
                                addClass(selectOptionTitleSpan, 'select__cta__title');
                                selectOptionTitleSpan.innerHTML = label;

                                // Appends
                                selectOptionButton.appendChild(selectOptionTitleSpan);
                            selectOption.appendChild(selectOptionButton);

                        // Assign to option group if available
                        if(closestOptGroup !== null){
                          var closestGeneratedOptGroup = selectList.querySelector('[data-label="' + closestOptGroup.getAttribute('label') + '"]')
                          closestGeneratedOptGroup.appendChild(selectOption);
                        }else{
                          selectList.appendChild(selectOption);
                        }

                        // Add to arrOptions array
                        arr.push(label);
                        i++;
                      }
                    });

      return [arr, uniqId, selectContainer, selectList];
  }

  /*
  * setNoResultsFoundElement()
  * Generate the "no results found" option
  */
  function setNoResultsFoundElement(state, selectField, selectContentList){
    switch(state){
      case "show":
        var prevEl = selectContentList.querySelector('[data-empty]');
        if(prevEl == null){
          var noResultsFoundElement = document.createElement('div');
              addClass(noResultsFoundElement, "select__item");

              selectContentList.appendChild(noResultsFoundElement);

              var noResultsFoundElementContent = document.createElement('div');
                  addClass(noResultsFoundElementContent, 'select__empty');
                  noResultsFoundElementContent.setAttribute('data-empty', '');
                  if(selectField.hasAttribute('data-search-empty')){
                    noResultsFoundElementContent.innerHTML = selectField.getAttribute('data-search-empty');
                  }else{
                    noResultsFoundElementContent.innerHTML = "No results found";
                  }

                  noResultsFoundElement.appendChild(noResultsFoundElementContent);
        }
      break;

      case "hide":
        var prevEl = selectContentList.querySelector('[data-empty]');
        if(prevEl !== null){
          removeElement(prevEl);
        }
      break;
    }
  }

  /*
  * setOriginalSelectFieldOption()
  * Setting the option in the hidden select field equal to the element selected in the generated select
  */
  function setOriginalSelectFieldOption(selectId, val){

    var sel = document.querySelector('.js-select[data-id="'+selectId+'"] select');
    for(var opt, j = 0; opt = sel.options[j]; j++) {
      if(opt.value == val) {
          sel.selectedIndex = j;
          return opt.label;
          break;
      }
    }
  }

})();
