/**
 * Progressively enhance an input field with a JS datepicker if possible
 */

// create empty datepicker object to attach global functions
vl.datepicker = {};
vl.datepicker.dress;
vl.datepicker.getDate;

// Keep supporting the older global vl.dressdatepicker function
vl.dressDatepicker = function() {
  var args = Array.prototype.slice.call(arguments).splice(1);
  var allArguments = args.concat(Array.prototype.slice.call(arguments));
  return vl.datepicker.dress.apply(this, allArguments);
};

(function () {

  var pickerFields = document.querySelectorAll('[data-datepicker]');
  var pickerFieldClass = 'input-field--datepicker';
  var pickerIconClass = 'datepicker__icon';
  var dateFormat = 'YYYY-MM-DD'; //'DD.MM.YYYY';

  var i18n = {
    previousMonth : 'Vorige maand',
    nextMonth     : 'Volgende maand',
    months        : ['januari','februari','maart','april','mei','juni','juli','augustus','september','oktober','november','december'],
    weekdays      : ['zondag','maandag','dinsdag','woensdag','donderdag','vrijdag','zaterdag'],
    weekdaysShort : ['zon','maa','din','woe','don','vri','zat']
  };

  vl.datepicker.dress = function(field) {
    var picker = new Pikaday({
      field: field,
      format: dateFormat,
      i18n: i18n,
      firstDay: 1,
      minDate: moment(field.getAttribute('data-datepicker-min'), dateFormat).toDate() || null,
      maxDate: moment(field.getAttribute('data-datepicker-max'), dateFormat).toDate() || null,
      yearRange: [moment(field.getAttribute('data-datepicker-min'), dateFormat).year(), moment(field.getAttribute('data-datepicker-max'), dateFormat).year()],
      showDaysInNextAndPreviousMonths: true
    });

    // add datepicker class
    addClass(field, pickerFieldClass);
    // if datepicker does not have an ID, add one
    if(!field.id){
      field.id = uniqueId();
    }
    // add datepicker label after the field
    var label = document.createElement('label');
    label.setAttribute('for', field.id);
    addClass(label, pickerIconClass);
    field.parentNode.appendChild(label);

    label.addEventListener('click', function (e) {
      e.preventDefault();
      picker.show();
    });
  };

  // create global function to get value from any datepicker field
  vl.datepicker.getDate = function(field) {
    return field.value;
  };

  // [].forEach.call(pickerFields, function(field) {
  //   vl.datepicker.dress(field);
  // });

})();
