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

/**
 * Progressively enhance an input field with a JS datepicker if possible
 */

/**
 * Progressively enhance a multiselect field
 */

// create global vl multiselect function
vl.multiselect = {};
vl.multiselect.dress;
vl.multiselect.setDisabledState;

(function () {

  var selectFields                      = document.querySelectorAll('[data-multiselect]'),
      selectContentListItemActiveState  = 'select__cta--active',
      selectContentListItemFocusState   = 'select__cta--focus',
      selectContentListItemHiddenState  = 'select__cta--hidden',
      lastSelectId, lastContainer;

  vl.multiselect.dress = function(selectField) {

    /*
    * Variables needed in Generate selects basted on original <select> elements
    */
    var arr                       = generateSelect(selectField),
        arrOptions                = arr[0],
        selectId                  = arr[1],
        selectContainer           = arr[2],
        activeArrOptions          = arrOptions, // = options that are shown
        selectDummyInput          = selectContainer.querySelector('.js-select__input'),
        selectContent             = selectContainer.querySelector('[data-content]'),
        selectContentInput        = selectContent.querySelector('[data-input]'),
        selectContentList         = selectContent.querySelector('[data-records]'),
        selectContentListItems    = selectContent.querySelectorAll('[data-record]'),
        selectFocusElems          = selectContainer.querySelectorAll('[data-focus]'),
        selectedArrOptions        = generatePills(selectField, selectContainer, selectDummyInput, selectContentListItems, selectId); // = options that are selected


    /*
    * Events in select element
    */
    //(selectContainer ? selectContainer.addEventListener('vlaanderen-multiselect-init', selectContainerInitEventHandler) : null );
    (selectContainer ? selectContainer.addEventListener('keyup', selectContainerKeyUpEventHandler) : null );
    (selectContainer ? selectContainer.addEventListener('keydown', selectContainerKeyDownEventHandler) : null );
    (selectDummyInput ? selectDummyInput.addEventListener('click', selectDummyInputClickEventHandler) : null );

    [].forEach.call(selectFocusElems, function(el){
      el.addEventListener('blur', selectFocusElemBlurHandler);
    });

    // function selectContainerInitEventHandler(e) {
    //   e.preventDefault();
    //   resetOptions();
    //   keyDefaultHandler();
    // }

    /*
    * Eventhandler | selectContainer keyUp
    */
    function selectContainerKeyUpEventHandler(e){
      e.preventDefault();

      var curOption, curOptionIndex;

      // Abort on tab and shift
      if(e.keyCode == 9 || e.keyCode == 16){
        return;
      }

      if(activeArrOptions.length <= 0){
        resetOptions();
      }else{
        setCurrentOption();
      }

      switch(e.keyCode){

        // "Backspace key"
        case 8:
          resetOptions();
          keyDefaultHandler();
        break;

        // "Esc key"
        case 27:
          setSelectState(false, selectContent, selectDummyInput);
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
        if(selectContent.getAttribute('data-show') !== "true"){
          setSelectState(true, selectContent, selectDummyInput);
          // Set focus
          if(selectContentInput !== null){
            selectContentInput.focus();
          }else{
            selectDummyInput.focus();
          }
        }else{
          curOption.click();
        }
        break;

        // "Arrow up key"
        case 38:
          if(activeArrOptions.length > 0)
            keyUpHandler();
        break;

        // "Arrow down key"
        case 40:
          if(activeArrOptions.length > 0)
            keyDownHandler();
        break;

        default:
          keyDefaultHandler();
        break;
      }
    }

    function keyUpHandler(){
      e.preventDefault();

      if(selectContent.getAttribute('data-show') !== "true"){
        // Tonen bij arrow down en index één verhogen zodat je op dezelfde positie zit bij het openen
        setSelectState(true, selectContent, selectDummyInput);
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

    function keyDownHandler(){
      e.preventDefault();

      if(selectContent.getAttribute('data-show') !== "true"){
        // Tonen bij arrow down en index één minderen zodat je op dezelfde positie zit bij het openen
        setSelectState(true, selectContent, selectDummyInput);
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

    function keyDefaultHandler(){
      if(selectContentInput !== null){
        var val = selectContentInput.value, first; activeArrOptions = [];

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
          setNoResultsFoundElement("hide", selectContainer, selectContentList);
          for(var opt, i = 0; opt = activeArrOptions[i]; i++) {
            selectContentList.querySelector('[data-record][data-label="'+opt+'"]').setAttribute('data-index', i);
          }
          // Set focus on first element
          if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
            selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');
        }else{
          setNoResultsFoundElement("show", selectContainer, selectContentList);
        }

      }
    }

    function setCurrentOption(){
      curOption = selectContentList.querySelector('[data-record][data-selected="true"]');
      (curOption == null ? curOption = selectContentList.querySelector('[data-record][data-index="0"]') : null);
      curOptionIndex = curOption.getAttribute('data-index');
    }

    function resetOptions(){
      for(var item, i = 0; item = arrOptions[i]; i++) {
        var el = selectContentList.querySelector('[data-record][data-label="'+item+'"]');
            el.removeAttribute('data-selected');
            el.setAttribute('data-show', 'true');
            el.setAttribute('data-index', i);
      }

      if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
        selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');

      activeArrOptions = arrOptions;
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
        setSelectState(true, selectContent, selectDummyInput);
        // Set focus on search if present
        if(selectContentInput !== null){
          selectContentInput.focus();
        }
        // Select first element in generate records
        if(selectContentList.querySelector('[data-record][data-index="0"]') !== null)
          selectContentList.querySelector('[data-record][data-index="0"]').setAttribute('data-selected','true');
      }
      else{
        setSelectState(false, selectContent, selectDummyInput);
      }
    }

    /*
    * selectFocusElemBlurHandler();
    * Used to check the focus state and close the select when focus is outside of the element
    */
    function selectFocusElemBlurHandler(e){
      window.setTimeout(function(){
        var parentElement = document.activeElement;
        if (parentElement) {
          var parent = parentElement.closest('.js-select[data-id="' + selectId + '"]');

          if (parent === null){
            setSelectState(false, selectContent, selectDummyInput);
          }
        }
      }, 200);
    };

    /*
    * Loop through dynamically generated records
    */
    [].forEach.call(selectContentListItems, function(item){
      var lbl = item.getAttribute('data-label');
      var val = item.getAttribute('data-value');

      item.addEventListener('click', function(e){
        // toggle active class to selected element
        toggleClass(item, selectContentListItemActiveState);

        // Set selected state to original select
        selectedArrOptions = setOriginalSelectFieldOptions(selectId, selectContentListItems, selectField);

        // Generate pills
        generatePills(selectField, selectContainer, selectDummyInput, selectContentListItems, selectId);

        // Set focus
        (selectContentInput !== null ? selectContentInput.focus() : selectDummyInput.focus());
      });
    });
  };

  /*
  * Loop through all select fields
  */
  // [].forEach.call(selectFields, function(selectField) {
  //   vl.multiselect.dress(selectField);
  // });

  /*
  * setDisabledState()
  * Sets disabled state of both native and generated select
  * @param selectField(object)
  * @param state(boolean)
  */
  vl.multiselect.setDisabledState = function(selectField, state) {
    var selectContainer   = selectField.closest('.js-select');
    var selectDummyInput  = selectContainer.querySelector('.js-select__input');

    if(state){
      selectField.setAttribute('disabled', state);
      selectDummyInput.setAttribute('disabled', state);
    }else{
      selectField.removeAttribute('disabled');
      selectDummyInput.removeAttribute('disabled');
    }
  };

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
  function setSelectState(isShown, selectContent, selectDummyInput){
    if(isShown)
      var dataShow = true, ariaHidden = false, ariaExpanded = true;
    else
      var dataShow = false, ariaHidden = true, ariaExpanded = false;

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
        selectDummyInput.setAttribute('data-focus', '');
        selectDummyInput.setAttribute('id', uniqId);
        selectDummyInput.setAttribute('aria-haspopup', 'true');
        selectDummyInput.setAttribute('aria-expanded', 'false');

        addClass(selectDummyInput, 'js-select__input');
        addClass(selectDummyInput, 'js-select__input--multi');
        if(selectField.hasAttribute('disabled') && selectField.getAttribute('disabled') !== "false")
          selectDummyInput.setAttribute('disabled','true');

        selectContainer.appendChild(selectDummyInput);

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
                    selectFormInputDescription.innerHTML = "U bevindt zich in de zoekfunctie van een dropdown menu met multiselect in een formulier. Navigeer door de opties met ctrl + alt + pijltjes en selecteer met enter. Gebruik escape om de dropdown te sluiten.";
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
                      if(!opt.hasAttribute('data-placeholder')){
                        // SelectOption
                        var selectOption = document.createElement('div');
                        addClass(selectOption, 'select__item');

                        // Titel (button wrapper)
                        var selectOptionButton = document.createElement('button');
                            addClass(selectOptionButton, 'select__cta');
                            // If option is selected set the element active and change the label in the DummyInput
                            if(opt.selected){
                              addClass(selectOptionButton, selectContentListItemActiveState);
                              selectOptionButton.setAttribute('aria-selected', true);
                            }else{
                              selectOptionButton.setAttribute('aria-selected', false);
                            }

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
                        var closestOptGroup = opt.closest('optgroup');
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

      return [arr, uniqId, selectContainer];
  }


  /*
  * generatePill()
  * Generating pills used in the multiselect input field
  */
  function generatePills(selectField, selectContainer, selectDummyInput, selectContentListItems, selectId){

    var activeOpts = selectContainer.querySelectorAll('.select__cta--active');
    var selectContentInput = selectContainer.querySelector('[data-input]');

    // Clear all elements
    selectDummyInput.innerHTML = "";

    // If present: generate pills based on active options
    if(activeOpts.length > 0){
      [].forEach.call(activeOpts, function(item){
          var pillWrapper = document.createElement("div");
          addClass(pillWrapper, 'pill'); addClass(pillWrapper, 'pill--closable');
          selectDummyInput.appendChild(pillWrapper);

            var pillSpan = document.createElement("span");
            pillSpan.innerHTML = item.getAttribute('data-label');
            pillWrapper.appendChild(pillSpan);

            var pillCta = document.createElement("a");
            addClass(pillCta, 'pill__close');
            pillCta.setAttribute('href', '#');
            pillCta.setAttribute('data-value', item.getAttribute('data-value'));
            pillCta.innerHTML = "close";

            // Remove pill on click/keyup(enter)
            pillCta.addEventListener('click', ctaEvent);
            pillCta.addEventListener('keyup', ctaKeydownEvent);

            function ctaKeydownEvent(e){
              if(e.keyCode === 13){
                ctaEvent(e);
              }
            }
            function ctaEvent(e){
              e.preventDefault();

              // Remove active class from corresponding item in select
              var correspondingItem = selectContainer.querySelector('[data-record][data-value="'+ item.getAttribute('data-value') +'"]');
              removeClass(correspondingItem, 'select__cta--active');

              // Generate pills
              generatePills(selectField, selectContainer, selectDummyInput, selectContentListItems, selectId);

              // Set selectfield options
              selectedArrOptions = setOriginalSelectFieldOptions(selectId, selectContentListItems, selectField);

              if(e.stopPropagation)
                e.stopPropagation();
              else
                e.cancelBubble = true;
            }
            pillWrapper.appendChild(pillCta);
      });
    }else{
      // Set placeholder or empty field
      var placeholder = selectField.querySelector('[data-placeholder]');
      if(placeholder !== null){
        selectDummyInput.innerHTML = placeholder.label;
      }else{
        selectDummyInput.innerHTML = "";
      }
    }
    return activeOpts;
  }

  /*
  * setNoResultsFoundElement()
  * Generate the "no results found" option
  */
  function setNoResultsFoundElement(state, selectContainer, selectContentList){
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
                  if(selectContainer.hasAttribute('data-search-empty')){
                    noResultsFoundElementContent.innerHTML = selectContainer.getAttribute('data-search-empty');
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
  * setOriginalSelectFieldOptions()
  * Setting the options in the hidden select field equal to the element selected in the generated select
  */
  function setOriginalSelectFieldOptions(selectId, selectContentListItems, selectField){
    var sel = document.querySelector('.js-select[data-id="'+selectId+'"] select');
    var selectedArrOptions = [];
    var values = [];

    [].forEach.call(selectContentListItems, function(item){
      if(hasClass(item, selectContentListItemActiveState)){
        selectedArrOptions.push(item);
        values.push(item.getAttribute('data-value'));
      }
    });

      var opts = sel.options;
      for (var i = 0; i < opts.length; i++)
      {
          opts[i].selected = false;
          for (var j = 0; j < values.length; j++)
          {
              if (opts[i].value == values[j])
              {
                  opts[i].selected = true;
                  break;
              }
          }
      }

      selectField.dispatchEvent(new CustomEvent('change', { bubbles: true }))

      // Return selected all options
      return selectedArrOptions;
  }
})();
