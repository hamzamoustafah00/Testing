// Short aliases:
// $ = document.querySelector
// $$ = document.querySelectorAll

export const $ = cssSelector => select(document, cssSelector);
export const $$ = cssSelector => select(document, cssSelector, true);

HTMLElement.prototype.$ = function (cssSelector) {
  return select(this, cssSelector);
}

HTMLElement.prototype.$$ = function (cssSelector) {
  return select(this, cssSelector, true);
}

// HTMLElement Query selector
function select(element, cssSelector, all = false) {
  let result = element['querySelector' + (all ? 'All' : '')](cssSelector);
  if (all) {
    // Get the NodeList and convert it to a real array
    // and add methods for delegated event handling to the array
    result = [...result];
    eventTypes.forEach(type => result[type] = handler =>
      addDelegatedEvent(type, cssSelector, handler));
  }
  return result;
}

// Simplify delegated eventhandling
// so we can do: $$('div').click(e => alert('I am a div'));

const eventTypes = Object.getOwnPropertyNames(window)
  .filter(x => x.slice(0, 2) === "on")
  .map(x => x.slice(2));

function addDelegatedEvent(type, cssSelector, handler) {
  document.body.addEventListener(type, event => {
    let closestEl = event.target.closest(cssSelector);
    if (!closestEl) { return; }
    handler(closestEl, event);
  });
}