// A very basic jQueryish mini-library
// for DOM element manipulation & delegated event handling
// © Ironboy 2024

export const $ = cssSelector => select(cssSelector);

HTMLElement.prototype.$ = function (cssSelector) {
  return select(cssSelector, this);
}

// HTMLElement Query selector
function select(cssSelector = null, element = document) {
  let elements = element.querySelectorAll(cssSelector);
  // Get the NodeList and convert it to a real array
  elements = [...elements];
  // Add some jQuery-inspired methods to the array
  addManipulationMethods(elements);
  // Add methods for delegated event handling to the array
  addDelegatedEventMethods(elements, cssSelector);
  return elements;
}

// Wrap an HTML Element in an elements array
function wrap(element) {
  const arr = select();
  arr.push(element);
  return arr;
}

// jQuery-inspired methods
function addManipulationMethods(elements) {
  const els = elements, methods = {
    html: set => set !== undefined ?
      els.forEach(x => x.innerHTML = set) :
      els.map(x => x.innerHTML),
    empty: () => html(''),
    attr: (name, set) => set !== undefined ?
      els.forEach(x => x.setAttribute(name, set)) :
      els.map(x => x.getAttribute(name)),
    addClass: _class =>
      els.forEach(x => x.classList.add(_class)),
    removeClass: _class =>
      els.forEach(x => x.classList.remove(_class)),
    toggleClass: _class =>
      els.forEach(x => x.classList.toggle(_class)),
    hasClass: _class =>
      els.any(x => x.classList.contains(_class)),
    eq: index => wrap(els[index])
  };

  // Add methods and chainability when a method
  // does not return anything to the elements array
  Object.entries(methods).forEach(([funcName, func]) =>
    els[funcName] = (...args) => {
      let result = func(...args);
      return result === undefined ? els : result;
    }
  );
}

// Simplify delegated eventhandling
// so we can do: $$('div').click(e => alert('I am a div'));

const eventTypes = Object.getOwnPropertyNames(window)
  .filter(x => x.slice(0, 2) === "on")
  .map(x => x.slice(2));

function addDelegatedEventMethods(elements, cssSelector) {
  eventTypes.forEach(type => elements[type] = handler =>
    addDelegatedEvent(type, cssSelector, handler));
}

function addDelegatedEvent(type, cssSelector, handler) {
  document.body.addEventListener(type, event => {
    let closestEl = event.target.closest(cssSelector);
    if (!closestEl) { return; }
    handler(wrap(closestEl), event);
  });
}