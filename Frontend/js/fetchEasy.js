import { marked } from "https://cdn.jsdelivr.net/npm/marked/lib/marked.esm.js";

export async function fetchEasy(url, options = {}) {
  let isText = !url.endsWith('.json') || !url.includes('.');
  let result = await (await fetch(url, options))[isText ? 'text' : 'json']();
  url.endsWith('.md') && (result = marked(result));
  isText && (result = await result.revive({
    import: async url =>
      await fetchEasy(url.includes('/') ? url : '/content/' + url)
  }, true).wait());
  return result;
}