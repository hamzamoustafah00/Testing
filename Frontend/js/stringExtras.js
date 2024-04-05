import "./jQueryish.js";

// Kebab case a string
// after removing diacritics from chars
String.prototype.kebabCase = function () {
  return this.normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replaceAll(' ', '-')
    .toLowerCase();
}

// Extract html from a string based on a cssSelector
String.prototype.extractHtml = function (cssSelector) {
  let tempDiv = document.createElement('div');
  tempDiv.innerHTML = this;
  let extracted = tempDiv.$(cssSelector).html();
  return extracted.length <= 1 ? extracted[0] : extracted;
}

// Replace {propName} in an html string 
// with property values from an object
// + if a property is a function we call it
// + if async = true, we can wait for async functions:
//   await "some string".revive(data,true).wait()
String.prototype.revive = function (data = {}, async = false) {
  let funcs = [];
  let result = this.replace(/\{([^\}]*)\}/g, prop => {
    prop = prop.slice(1, -1)
      .replaceAll('&quot;', '"').replaceAll('&#39;', "'");
    funcs.push(new Function('data', `
      let x = data.${prop};
      return x === undefined ? \`{${prop}}\` : x
    `));
    return '__result__' + (funcs.length - 1);
  });
  let funcResults = funcs.map(x => x(data));
  let resolver = () => result.replace(/__result__\d{1,}/g, x =>
    funcResults[+x.split('_').pop()]);
  if (!async) { return resolver(); }
  else {
    let s = new String(result);
    s.wait = async () => {
      for (let i = 0; i < funcResults.length; i++) {
        funcResults[i] = await funcResults[i];
      }
      return resolver();
    }
    return s;
  }
}