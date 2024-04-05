import { fetchEasy } from "./fetchEasy.js";

// Kebab case a string
// after removing diacritics from chars
String.prototype.kebabCase = function () {
  return this.normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replaceAll(' ', '-')
    .toLowerCase();
}

// Split an html string based on a headline level
String.prototype.splitHtml = function (headlineLevel) {
  let on = `<h${headlineLevel}>`;
  return this.split(on).slice(1).map(part => on + part);
}

// Replace {propName} in an html string 
// with property values from an object
// + if a property is a function you we call it
// + if you call async with async = true, you can 
//   chain in a call to wait to resolve async function calls
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