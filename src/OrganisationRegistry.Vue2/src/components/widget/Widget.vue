<template>
  <div v-show="resolved"/>
</template>

<script>

import axios from 'axios'

/**
 * Add a collection of stylesheet definitions.
 *
 * @param {Object[]} defs
 *   An array of stylesheet definitions.
 * @param {Function} callback
 *   A function which will be invoked when collection
 *   is processed.
 */
function addStyleSheets (defs, callback) {
  // Set the counter equal to the number of definitions. This
  // counter will be decremented until zero is reached. when
  // zero is reached we will invoke the callback.
  let counter = defs.length
  // Iterate through the collection of stylesheet definitions.
  for (const def of defs) {
    // Add the StyleSheet definition.
    addStylesheet.call(this, def, () => {
      // Decrement the counter and check wheher zero is reached.
      if ((--counter) === 0 && callback) {
        // Invoke the callback as we completed resolving the
        // stylesheets.
        callback()
      }
    })
  }
}

/**
 * Add a stylesheet.
 *
 * @param {Object} def
 *   An object which contains the following properties:
 *   <ul>
 *     <li>data: Contains the Stylesheet data</li>
 *     <li>defer: Flag which indicates whether deferred behavior is supported</li>
 *     <li>type: Type of reference.</li>
 *   </ul>
 * @param {Function} callback
 *   A function which will be invoked when stylesheet is completed.
 */
function addStylesheet (def, callback) {
  // Evaluate the type of definition.
  switch (def.type || '') {
    case 'external':
      // Get the head element by name.
      const head = document.getElementsByTagName('head')[0]
      // Create a stylesheet element.
      const styleElement = document.createElement('style')
      // Configure the style element type attributes.
      styleElement.setAttribute('type', 'text/css')
      // Build thes style element content.
      const styleContent = '@import url("' + def.data + '");'
      // Set the default property names.
      let sheetProperty = 'sheet'
      let cssRulesProperty = 'cssRules'
      // Check if the styleElement uses IE like properties.
      if (styleElement.styleSheet) {
        // Append the style elemnt to the document head. Note: style
        // needs to be attached to the head before setting the text else
        // IE6-8 will crash.
        head.appendChild(styleElement)
        // Set the style element content.
        styleElement.styleSheet.cssText = styleContent
        // Change the default property names to IE specified values.
        sheetProperty = 'styleSheet'
        cssRulesProperty = 'rules'
      } else {
        // Create the style content element.
        const styleContentElement = document.createTextNode(styleContent)
        // Append the style content element.
        styleElement.appendChild(styleContentElement)
        // Append the style element to the head.
        head.appendChild(styleElement)
      }
      // Check whether a callback was provided.
      if (callback) {
        // Setup polling which will check whether stylesheet is loaded.
        const interval = setInterval(() => {
          try {
            // Accessing this property will raise an error when stylesheet
            // is not loaded yet.
            styleElement[sheetProperty][cssRulesProperty]
            // Clear the interval timer.
            clearInterval(interval)
            // Invoke the callback.
            callback()
          } catch (error) { /* Ignore error */ }
        }, 10)
      }
      break

    default:
      // Raise error due to unknown stylesheet type.
      throw new Error('Unknown stylesheet definition type')
  }
}

/**
 * Add a collection of scripts.
 *
 * @param {Object[]} defs
 *   An array of script definitions.
 * @param {Function} callback
 *   A function which will be invoked when collection
 *   is processed.
 */
function addScripts (defs, callback) {
  // Check whether at least one item is present in the collection.
  if (defs.length > 0) {
    // Get a reference to ourself.
    const self = this
    // Create function will process to the remaining scripts.
    const processRemainer = () => { addScripts.call(self, defs.slice(1), callback) }
    // Check whether script can be deferred.
    if (defs[0].defer) {
      // Add the first item of the list.
      addScript.call(this, defs[0])
      // Queue the remaining scripts.
      processRemainer()
    } else {
      // Add the specified script definition.
      addScript.call(this, defs[0], processRemainer)
    }
  } else if (callback) {
    // Invoke the callback as all scripts have been resolved.
    callback()
  }
}

/**
 * Add a script.
 *
 * @param {Object} def
 *   An object which contains the following properties:
 *   <ul>
 *     <li>data: Contains the script data</li>
 *     <li>defer: Flag which indicates whether deferred behavior is supported</li>
 *     <li>type: Type of reference.</li>
 *   </ul>
 * @param {Function} callback
 *   A function which will be invoked when script is completed.
 */
function addScript (def, callback) {
  // Evaluate the type of definition.
  switch (def.type || '') {
    case 'external':
      // Create a script element.
      const script = document.createElement('script')
      // Check whether a callback function is specified.
      if (callback) {
        // Check if the readyState property is supported (IE).
        if (script.readyState) {
          // Attach an event handler to the "onreadystatechange" event.
          script.onreadystatechange = () => {
            // Check if the script is loaded or complete.
            if (script.readyState === 'loaded' || script.readyState === 'complete') {
              // Remove the event handler.
              script.onreadystatechange = null
              // Invoke the callback.
              callback()
            }
          }
        } else {
          // Attach an event handler to the "onload" event.
          script.onload = () => { callback() }
        }
      }
      // Configure the script element attributes.
      script.setAttribute('type', 'text/javascript')
      script.setAttribute('src', def.data)
      script.setAttribute('async', 'true')
      // Append the element to the head.
      document.getElementsByTagName('head')[0].appendChild(script)
      break

    default:
      // Raise error due to unknown script type.
      throw new Error('Unknown script definition type')
  }
}

export default {
  data () {
    return {
      resolved: false
    }
  },
  props: {
    id: {
      type: String,
      required: true
    },
    lang: {
      type: String,
      required: true
    },
    endpoint: {
      type: String,
      required: false,
      default: 'widgets.vlaanderen.be'
    },
    protocol: {
      type: String,
      required: false,
      default: 'https'
    }
  },
  beforeMount () {
    // Ensure the WidgetApi.Event.Handlers namespace is available.
    window.WidgetApi = window.WidgetApi || {}
    window.WidgetApi.Event = window.WidgetApi.Event || {}
    window.WidgetApi.Event.Handlers = window.WidgetApi.Event.Handlers || []
  },
  mounted () {
    // Get a reference to ourself.
    const self = this
    // Build a temporary store which holds the asset resolved state.
    const resolvedState = { css: false, js: false, attached: false }

    /**
     * Update the resolve state for given asset type.
     *
     * @param {String} type
     *   Type of asset which has been resolved.
     */
    function updateResolvedState (type) {
      // Update the resolved state for given type.
      resolvedState[type] = true
      // Determine whether all assets have been resolved.
      self.resolved = resolvedState.css && resolvedState.js && resolvedState.attached
    }

    // Register an event hanlder which will detect when the widget is created. This
    // is to keep the language of the widget in sync with the configured language
    // on the component.
    window.WidgetApi.Event.Handlers.push({
      event: 'WidgetCreated',
      data: this,
      handler: (event) => {
        // Get the widget from the event source.
        const widget = event.getEventArgs().getSource()
        // Check whether the attached widget matches our component.
        if (widget.id() === self.id && widget.getLanguage() !== self.lang) {
          // Overwrite the language resolve method to match our component
          // language.
          widget.getLanguage = () => self.lang
        }
      }
    })

    // Register an event handler which will detect when the widget is attachd. This
    // is to prevent flickering behavior.
    window.WidgetApi.Event.Handlers.push({
      event: 'WidgetAttached',
      data: this,
      handler: (event) => {
        // Get the widget from the event source.
        const widget = event.getEventArgs().getSource()
        // Check whether the attached widget matches our component.
        if (widget.id() === self.id) {
          // Update the attached state.
          updateResolvedState('attached')
        }
      }
    })

    axios
      // Request the Widget related assets and content from the REST API server.
      .get(this.protocol + '://' + this.endpoint + '/widget/smart-ssi/' + this.id + '?lang=' + this.lang)
      .then((res) => {
        // Add the specified set of stylesheets.
        addStyleSheets.call(this, res.data.assets.css || [], () => { updateResolvedState('css') })
        // Add the specified set of scripts.
        addScripts.call(this, res.data.assets.js || [], () => { updateResolvedState('js') })
        // Attach the Widget content as inner HTML for our Vue component.
        this.$el.innerHTML = res.data.content
      })
  },
  render (createElement) {
    // Render the component default slot as long as the widget is not resolved.
    return createElement('div', this.resolved ? undefined : this.$slots.default)
  }
  // @todo Provide watch on different properties which will reset the resolved state.
}
</script>
