<template>
  <div class="u-table-overflow">
    <table :class="['data-table', css.tableClass]">
      <thead>
        <tr class="data-table__header">
          <template v-for="field in tableFields">
            <template v-if="field.visible">
              <template v-if="isSpecialField(field.name)">
                <th v-if="extractName(field.name) == '__checkbox'"
                    :key="field.name"
                    :class="['data-table__header-title', 'data-table-th-checkbox-' + trackBy, field.titleClass]"
                    style="width: 4%;">
                  <label class="checkbox checkbox--empty" for="checkbox-check-all">
                    <input type="checkbox"
                           id="checkbox-check-all"
                           name="checkbox"
                           class="checkbox__toggle"
                           value="2"
                           @change="toggleAllCheckboxes($event)"
                           :checked="checkCheckboxesState()">
                    <span></span> <span class="u-visually-hidden">Selecteer alle rijen</span> </label>
                </th>
                <th v-if="extractName(field.name) == '__component'"
                    :key="field.name"
                    @click="orderBy(field, $event)"
                    :class="['data-table-th-component-' + trackBy, field.titleClass, {'sortable': isSortable(field)}]"
                    v-html="renderTitle(field)"
                    :style="[renderWidthStyle(field)]">
                ></th>
                <th v-if="extractName(field.name) == '__slot'"
                    :key="field.name"
                    @click="orderBy(field, $event)"
                    :class="['data-table-th-slot-' + extractArgs(field.name), field.titleClass, {'sortable': isSortable(field)}]"
                    v-html="renderTitle(field)"
                    :style="[renderWidthStyle(field)]">
                ></th>
                <th v-if="extractName(field.name) == '__sequence'"
                    :key="field.name"
                    :class="['data-table-th-sequence', field.titleClass || '']"
                    v-html="renderTitle(field)"
                    :style="[renderWidthStyle(field)]">
                </th>
                <th v-if="notIn(extractName(field.name), ['__sequence', '__checkbox', '__component', '__slot'])"
                    :key="field.name"
                    :class="['data-table-th-' + field.name, field.titleClass || '', {'sortable': isSortable(field)}]"
                    v-html="renderTitle(field)"
                    :style="[renderWidthStyle(field)]">
                </th>
              </template>
              <template v-else>
                <th :key="field.name"
                    :id="'_' + field.name"
                    :class="['data-table__header-title', 'data-table-th-' + field.name, field.titleClass, {'sortable': isSortable(field)}]"
                    @click="orderBy(field, $event)"
                    :style="[renderWidthStyle(field)]">
                  <a v-if="isSortable(field)"
                     class="data-table__header-title--sortable data-table__header-title--sortable-active"
                     :title="sortIcon(field)"
                     v-html="renderTitle(field)">
                  </a>
                  <span v-else
                        v-html="renderTitle(field)">
                  </span>
                </th>
              </template>
            </template>
          </template>
        </tr>
      </thead>
      <tbody v-cloak class="data-table-body">
        <template v-for="(item, index) in tableData">
          <tr @dblclick="onRowDoubleClicked(item, $event)"
              @click="onRowClicked(item, $event)"
              :key="index"
              :item-index="index"
              :render="onRowChanged(item)"
              :class="onRowClass(item, index)">
            <template v-for="field in tableFields">
              <template v-if="field.visible">
                <template v-if="isSpecialField(field.name)">
                  <td v-if="extractName(field.name) == '__checkbox'"
                      :class="['data-table-checkboxes', 'data-table-td-checkbox-' + trackBy, field.dataClass]"
                      :key="field.name">
                    <label class="checkbox checkbox--empty">
                      <input type="checkbox"
                             name="checkbox"
                             class="checkbox__toggle"
                             value="2"
                             @change="toggleCheckbox(item, $event)"
                             :checked="rowSelected(item)">
                      <span></span> <span class="u-visually-hidden">Selecteer rij</span> </label>
                  </td>
                  <td v-if="extractName(field.name) === '__component'"
                      :class="['data-table-component', field.dataClass]"
                      :key="field.name">
                    <component :is="extractArgs(field.name)"
                               :row-data="item"
                               :row-index="index"
                               :row-field="field.sortField"
                    ></component>
                  </td>
                  <td v-if="extractName(field.name) === '__slot'"
                      :class="['data-table-slot', field.dataClass]"
                      :key="field.name">
                    <slot :name="extractArgs(field.name)"
                          :row-data="item"
                          :row-index="index"
                          :row-field="field.sortField"
                    ></slot>
                  </td>
                  <td v-if="extractName(field.name) == '__sequence'"
                      :class="['data-table-sequence', field.dataClass]"
                      :key="field.name"
                      v-html="renderSequence(index)">
                  </td>
                  <td v-if="extractName(field.name) == '__handle'"
                      :class="['data-table-handle', field.dataClass]"
                      :key="field.name"
                      v-html="renderIconTag(['handle-icon', css.handleIcon])"
                  ></td>
                </template>
                <template v-else>
                  <td v-if="hasCallback(field)"
                      :class="field.dataClass"
                      :key="field.name"
                      @click="onCellClicked(item, field, $event)"
                      @dblclick="onCellDoubleClicked(item, field, $event)"
                      v-html="callCallback(field, item)"
                  ></td>
                  <td v-else
                      :class="field.dataClass"
                      :key="field.name"
                      @click="onCellClicked(item, field, $event)"
                      @dblclick="onCellDoubleClicked(item, field, $event)"
                      v-html="getObjectValue(item, field.name, '')"
                  ></td>
                </template>
              </template>
            </template>
          </tr>
        </template>
        <template v-if="displayLoadingMessage">
          <tr>
            <td :colspan="countVisibleFields" class="data-table-empty-result">{{loadingMessage}}</td>
          </tr>
        </template>
        <template v-else-if="displayEmptyDataRow">
          <tr>
            <td :colspan="countVisibleFields" class="data-table-empty-result">{{noDataTemplate}}</td>
          </tr>
        </template>
        <template v-if="lessThanMinRows">
          <tr v-for="i in blankRows" class="blank-row" :key="i">
            <template v-for="field in tableFields">
              <td v-if="field.visible" :key="field.name">&nbsp;</td>
            </template>
          </tr>
        </template>
      </tbody>
    </table>
    <div
        class="data-table__actions data-table__actions--bottom"
      >
        <div class="pager">
          <ul class="pager__list pager__list--right">
            <li class="pager__element">
              <span><strong>{{paginatedItems.from}} - {{paginatedItems.to}}</strong> van {{pagination.totalItems}}</span>
            </li>
            <li
              v-if="allowNavigatePrevious()"
              class="pager__element"
            >
              <a
                @click="gotoPreviousPage()"
                class="link-text"
              >
                <i class="vi vi-arrow previous vi-u-s">vorige</i>
              </a>
            </li>
            <li
              v-if="allowNavigateNext()"
              class="pager__element"
            >
              <a
                @click="gotoNextPage()"
                class="link-text"
                >
                  <i class="vi vi-arrow next vi-u-s">volgende</i>
              </a>
            </li>
          </ul>
        </div>
      </div>
  </div>



</template>

<script>
// import Field from './fields';
// import Css from './css';

// declare var vl; // global vlaanderen ui

/* eslint-disable */

const SORT = {
  Ascending : 'ascending',
  Descending: 'descending',
};

export default {
  name: 'data-table',
  props: {
    css: {
      type: Object,
      default() {
        return {
          tableClass: '',
          loadingClass: 'loading',
          ascendingIcon: '',
          ascendingText: 'Aflopend sorteren',
          descendingIcon: 'vi-u-180deg',
          descendingText: 'Oplopend sorteren',
          detailRowClass: 'data-table-detail-row',
          handleIcon: 'grey sidebar icon',
        };
      },
    },
    rowClass: {
      type: [String, Function],
      default: ''
    },
    fields: {
      type: Array,
      required: true,
    },
    noDataTemplate: {
      type: String,
      default: 'Geen data beschikbaar...',
    },
    loadingMessage: {
      type: String,
      default: 'Bezig met laden...',
    },
    dataPath: {
      type: String,
      default: 'data',
    },
    perPage: {
      type: Number,
      default: 10,
    },
    sortColumn: {
      type: Object,
      required: true,
    },
    pagination: {
      type: Object,
      required: true,
    },
    data: {
      type: [Array, Object],
      default: null,
    },
    dataManager: {
      type: Function,
      default: null,
    },
    silent: {
      type: Boolean,
      default: false,
    },
    trackBy: {
      type: String,
      default: 'id',
    },
    minRows: {
      type: Number,
      default: 0,
    },
    isLoading: {
      type: Boolean,
      required: true,
    }
  },
  data () {
    return {
      eventPrefix: 'vuetable:',
      tableFields: [],
      tableData: null,
      selectedTo: [],
      visibleDetailRows: [],
      lastScrollPosition: 0,
      scrollBarWidth: '17px', //chrome default
      scrollVisible: false,
    }
  },
  computed: {
    countVisibleFields() {
      return this.tableFields.filter(function(field) {
        return field.visible;
      }).length;
    },
    countTableData() {
      return (this.tableData || []).length;
    },
    displayEmptyDataRow() {
      return this.countTableData === 0 && this.noDataTemplate.length > 0;
    },
    displayLoadingMessage() {
      return this.isLoading && this.loadingMessage.length > 0;
    },
    lessThanMinRows() {
      return this.countTableData < this.minRows;
    },
    blankRows() {
      return this.lessThanMinRows ? (this.minRows - this.countTableData) : 0;
    },
    paginatedItems() {
        const {
          offset = 0,
          totalItems = 0,
          limit = 10
        } = this.pagination;

        return {
          from: totalItems === 0 ? 0 : offset + 1,
          to: Math.min(offset + limit, totalItems),
        };
    },
  },
  methods: {
    normalizeFields() {
      if (typeof (this.fields) === 'undefined') {
        this.warn('You need to provide "fields" prop.');
        return;
      }

      this.tableFields = [];
      let self = this;
      let obj;

      this.fields.forEach((field, i) => {
        if (typeof (field) === 'string') {
          obj = {
            name,
            title: self.setTitle(field),
            sortField,
            titleClass: '',
            dataClass: '',
            callback: undefined,
            visible: true,
            widthPercentage: undefined,
          }
        } else {
          obj = {
            name: field.name,
            title: (field.title === undefined) ? self.setTitle(field.name) : field.title,
            sortField: field.sortField,
            titleClass: (field.titleClass === undefined) ? '' : field.titleClass,
            dataClass: (field.dataClass === undefined) ? '' : field.dataClass,
            callback: (field.callback === undefined) ? '' : field.callback,
            visible: (field.visible === undefined) ? true : field.visible,
            widthPercentage: (field.widthPercentage === undefined) ? 0 : field.widthPercentage,
          }
        }

        self.tableFields.push(obj);
      })
    },
    setData(data) {
      if (Array.isArray(data)) {
        this.tableData = data;
        return;
      }

      this.fireEvent('loading');

      this.tableData = this.getObjectValue(data, this.dataPath, null);

      this.$nextTick(function () {
        this.fireEvent('loaded');
      });
    },
    callRefreshData({ sorting, pagination = {} }) {
      if (this.dataManager === null && this.data === null)
        return;

      const listSorting = sorting || { ...this.sortColumn };
      const paging = {
          offset: pagination.offset != null ? pagination.offset : this.pagination.offset,
          limit: pagination.limit || this.pagination.limit,
      }

      this.dataManager(listSorting, paging);
    },
    transform(data) {
      let func = 'transform';

      if (this.parentFunctionExists(func)) {
        return this.$parent[func].call(this.$parent, data);
      }

      return data;
    },
    parentFunctionExists(func) {
      return (func !== '' && typeof this.$parent[func] === 'function')
    },
    setTitle(str) {
      if (this.isSpecialField(str)) {
        return '';
      }

      return this.titleCase(str);
    },
    renderTitle(field) {
      let title = (typeof field.title === 'undefined') ? field.name.replace('.', ' ') : field.title;

      if (title.length > 0 && this.isSorted(field)) {
        return title + this.renderSortHint(field) + this.renderIconTag(['data-table__header-title__sort-icon', 'vi', 'vi-u-link-after', 'vi-long-arrow', 'sort-icon', this.sortIcon(field)]);
      }

      return title;
    },
    renderWidthStyle(field) {
      let widthPercentage = (typeof field.widthPercentage === 'undefined') ? 0 : field.widthPercentage;

      return widthPercentage !== 0
        ? { width: `${widthPercentage}%` }
        : { width: 'auto' };
    },
    renderSortHint(field) {
      let sortHint = this.sortIcon(field);

      return (sortHint !== '')
        ? `<span class="u-visually-hidden">${sortHint}</span>`
        : '';
    },
    renderIconTag(classes, options = '') {
      return `<i class="${classes.join(' ')}" ${options}></i>`;
    },
    isSpecialField(fieldName) {
      return fieldName.slice(0, 2) === '__';
    },
    titleCase(str) {
      return str.replace(/\w+/g, function(txt) {
        return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
      });
    },
    camelCase(str, delimiter = '_') {
      let self = this;
      return str.split(delimiter).map(function(item) {
        return self.titleCase(item);
      }).join('');
    },
    notIn(str, arr) {
      return arr.indexOf(str) === -1;
    },
    extractName(str) {
      return str.split(':')[0].trim();
    },
    extractArgs(str) {
      return str.split(':')[1];
    },
    getObjectValue(object, path, defaultValue) {
      defaultValue = (typeof defaultValue === 'undefined') ? null : defaultValue;

      let obj = object;
      if (path.trim() !== '') {
        let keys = path.split('.');

        keys.forEach(function(key) {
          if (obj !== null && typeof obj[key] !== 'undefined' && obj[key] !== null) {
            obj = obj[key];
          } else {
            obj = defaultValue;
            return;
          }
        })
      }

      return obj;
    },
    isSortable(field) {
      return !(typeof field.sortField === 'undefined');
    },
    orderBy(field, event) {
      if (!this.isSortable(field))
        return;

      const sorting = {
        sortField: (field.sortField || '').toLowerCase(),
        direction: (this.isSorted(field) && this.sortColumn.direction === SORT.Ascending) ? SORT.Descending : SORT.Ascending
      };

      this.callRefreshData({ sorting, pagination: { limit: this.pagination.limit, offset: 0 } });
    },
    sortIcon(field) {
      if(this.isSorted(field)) {
        return this.sortColumn.direction === SORT.Ascending ? this.css.ascendingIcon : this.css.descendingIcon;
      }
      return '';
    },
    isSorted(field = {}){
      return this.sortColumn.sortField === (field.sortField|| '').toLowerCase();
    },
    hasCallback(field) {
      return field.callback ? true : false;
    },
    callCallback(field, item) {
      if (!this.hasCallback(field)) {
        return;
      }

      if (typeof (field.callback) == 'function') {
        return field.callback(this.getObjectValue(item, field.name));
      }

      const args = (field.callback || '').split('|');
      const func = args.shift();

      if (typeof this.$parent[func] === 'function') {
        const value = this.getObjectValue(item, field.name);

        return (args.length > 0)
          ? this.$parent[func].apply(this.$parent, [value].concat(args))
          : this.$parent[func].call(this.$parent, value);
      }

      return null;
    },
    toggleCheckbox(dataItem, event) {
      let isChecked = event.target.checked;
      let idColumn = this.trackBy;

      if (dataItem[idColumn] === undefined) {
        this.warn('__checkbox field: The "' + this.trackBy + '" field does not exist! Make sure the field you specify in "track-by" prop does exist.')
        return
      }

      let key = dataItem[idColumn];
      if (isChecked) {
        this.selectId(key);
      } else {
        this.unselectId(key);
      }

      this.fireEvent('checkbox-toggled', isChecked, dataItem);
    },
    selectId(key) {
      if (!this.isSelectedRow(key)) {
        this.selectedTo.push(key);
      }
    },
    unselectId(key) {
      this.selectedTo = this.selectedTo.filter(function(item) {
        return item !== key;
      })
    },
    isSelectedRow(key) {
      return this.selectedTo.indexOf(key) >= 0;
    },
    rowSelected(dataItem){
      let idColumn = this.trackBy;
      let key = dataItem[idColumn];

      return this.isSelectedRow(key);
    },
    checkCheckboxesState() {
      if (!this.tableData)
        return;

      let self = this;
      let idColumn = this.trackBy;
      let selector = 'th.data-table-th-checkbox-' + idColumn + ' input[type=checkbox]';
      let els = document.querySelectorAll(selector);

      // fixed:document.querySelectorAll return the typeof nodeList not array
      if ((els).forEach === undefined)
        (els).forEach = function(cb) {
          [].forEach.call(els, cb);
        }

      // count how many checkbox row in the current page has been checked
      let selected = this.tableData.filter(function(item) {
        return self.selectedTo.indexOf(item[idColumn]) >= 0;
      });

      // count == 0, clear the checkbox
      if (selected.length <= 0) {
        (els).forEach(function(el) {
          el.indeterminate = false;
        });

        return false;
      }

      // count > 0 and count < perPage, set checkbox state to 'indeterminate'
      else if (selected.length < this.perPage && selected.length !== this.tableData.length) {
        (els).forEach(function(el) {
          el.indeterminate = true;
        });

        return false;
      }
      // count == perPage, set checkbox state to 'checked'
      else {
        (els).forEach(function(el) {
          el.indeterminate = false;
        })

        return true;
      }
    },
    toggleAllCheckboxes(event) {
      let self = this;
      let isChecked = event.target.checked;
      let idColumn = this.trackBy;

      if (isChecked) {
        this.tableData.forEach(function(dataItem) {
          self.selectId(dataItem[idColumn]);
        })
      } else {
        this.tableData.forEach(function(dataItem) {
          self.unselectId(dataItem[idColumn]);
        })
      }

      this.fireEvent('checkbox-toggled-all', isChecked);
    },
    allowNavigatePrevious(){
      return this.pagination.offset > 0;
    },
    allowNavigateNext(){
      return this.pagination.offset + this.pagination.limit < this.pagination.totalItems;
    },
    gotoPreviousPage() {
      if (this.allowNavigatePrevious()) {
        this.callRefreshData({ pagination: {
          offset:this.pagination.offset - this.pagination.limit,
          limit: this.pagination.limit
        }});
      }
    },
    gotoNextPage() {
      if (this.allowNavigateNext()) {
        this.callRefreshData({ pagination: {
          offset:this.pagination.offset + this.pagination.limit,
          limit: this.pagination.limit
        }});
      }
    },
    gotoPage(page) {
      if (page !== this.pagination.currentPage && page > 0 && page <= this.pagination.totalPages) {
        this.callRefreshData({ page });
      }
    },
    showField(index) {
      if (index < 0 || index > this.tableFields.length)
        return;

      this.tableFields[index].visible = true;
    },
    hideField(index) {
      if (index < 0 || index > this.tableFields.length)
        return;

      this.tableFields[index].visible = false;
    },
    toggleField(index) {
      if (index < 0 || index > this.tableFields.length)
        return;

      this.tableFields[index].visible = ! this.tableFields[index].visible;
    },
    warn (msg) {
      if (!this.silent)
        console.warn(msg);
    },
    fireEvent(eventName, ...args) {
      this.$emit(this.eventPrefix + eventName, args);
    },
    onRowClass(dataItem, index) {
      if (typeof (this.rowClass) === 'function') {
        return this.rowClass(dataItem, index);
      }

      return this.rowClass;
    },
    onRowChanged(dataItem) {
      this.fireEvent('row-changed', dataItem);
      return true;
    },
    onRowClicked(dataItem, event) {
      this.fireEvent('row-clicked', dataItem, event);
      return true;
    },
    onRowDoubleClicked (dataItem, event) {
      this.fireEvent('row-dblclicked', dataItem, event);
    },
    onDetailRowClick (dataItem, event) {
      this.fireEvent('detail-row-clicked', dataItem, event);
    },
    onCellClicked(dataItem, field, event) {
      this.fireEvent('cell-clicked', dataItem, field, event);
    },
    onCellDoubleClicked(dataItem, field, event) {
      this.fireEvent('cell-dblclicked', dataItem, field, event);
    },

    // API for externals/manual calls from components
    changePage(page) {
      if (['prev', 'previous'].includes(page)) {
        this.gotoPreviousPage();
      } else if (page === 'next') {
        this.gotoNextPage();
      } else if (typeof page === 'number') {
        this.gotoPage(page);
      }
    },
  },
  mounted() {
    this.normalizeFields();
    this.$nextTick(function () {
      this.fireEvent('initialized', this.tableFields);
    });

    this.callRefreshData({});
  },
};
</script>

<style scoped>
[v-cloak] {
  display: none;
}

.data-table th.sortable:hover {
  cursor: pointer;
}

.data-table-actions {
  width: 15%;
  padding: 12px 0px;
  text-align: center;
}

.data-table-pagination {
  background: #f9fafb !important;
}

.data-table-pagination-info {
  margin-top: auto;
  margin-bottom: auto;
}

.data-table-empty-result {
  text-align: center;
}

.link-text {
  cursor: pointer;
}

.vi-arrow:before,
.vi-arrow:after {
  /* background-color: aquamarine; */

  font-size: 13px;
  vertical-align: middle;

  content: "";
  font-family: "vlaanderen_iconfont";
  display: inline-block;
}

.vi-arrow.previous:before,
.vi-arrow.next:after {
  content: "\E004";
  padding-left: 0.2em;
  -moz-transform: scale(1, 1);
  -webkit-transform: scale(1, 1);
  -o-transform: scale(1, 1);
  -ms-transform: scale(1, 1);
  transform: scale(1, 1);
}

.vi-arrow.previous:before {
  -moz-transform: scale(-1, -1);
  -webkit-transform: scale(-1, -1);
  -o-transform: scale(-1, -1);
  -ms-transform: scale(-1, -1);
  transform: scale(-1, -1);
}

.vi-arrow.next:after {
  -moz-transform: scale(1, 1);
  -webkit-transform: scale(1, 1);
  -o-transform: scale(1, 1);
  -ms-transform: scale(1, 1);
  transform: scale(1, 1);
}

</style>
