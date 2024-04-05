import { marked } from "https://cdn.jsdelivr.net/npm/marked/lib/marked.esm.js";

export async function fetchEasy(url, options = {}) {
  let isText = !url.endsWith('.json') || !url.includes('.');
  let result = await (await fetch(url, options))[isText ? 'text' : 'json']();
  url.endsWith('.md') && (result = marked(result));
  isText && (result = await result.revive({
    import: async url =>
      await fetchEasy(url.includes('/') ? url : '/content/' + url)
  }, true).wait());
  if (url.endsWith('.md') || url.endsWith('.html')) {
    result = `<div id="${url.split('/').pop().split('.')[0]}">\n${result}\n</div>`;
  }
  return result;
}