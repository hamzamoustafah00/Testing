import { marked } from "https://cdn.jsdelivr.net/npm/marked/lib/marked.esm.js";

// Wrap document.querySelector in a function so we can call it using $
const $ = cssSelector => document.querySelector(cssSelector);

// Remove diacritics (åäö etc) and kebab case a string
const kebabCase = str => str
  .normalize("NFD")
  .replace(/[\u0300-\u036f]/g, "")
  .replaceAll(' ', '-')
  .toLowerCase();

// Read content from the markdown file content.md 
// and convert it from markdown to HTML
// and split it into parts - one per h1
const content =
  marked(await (await fetch('content.md')).text())
    .split('<h1>').slice(1).map(part => '<h1>' + part);

// Extract the h1 content as menu items
const menuItems = content.map(x => x.slice(4).split('<')[0]);

// Add initial html for the site
$('body').innerHTML = /*html*/`
  <header>
    <nav>
    <div class="logo">The Awesome Company</div>
    ${menuItems.map((item, index) => /*html*/`
      <a href="/${index === 0 ? '' : kebabCase(item)}">${item}</a>
    `).join('')}
    </nav>
  </header>
  <main>
    <article></article>
  </main>
  <footer>
    <section>
      Email: <a href="#">awesome@gmail.com</a>
      <div class="right">
        <a href="/">© 2024 <span class="logo">The Awesome Company</span></a>
      </div>
    </section>
  </footer>
`;

// When we click somewhere - check if the click
// sis on an a tag with an internal link
$('body').addEventListener('click', e => {
  // Check that we have clicked an a tag
  let aTag = e.target.closest('a');
  if (!aTag) { return; }
  // Check that the link is internal (starts with '/')
  let href = aTag.getAttribute('href');
  if (!href[0] === '/') { return; }
  // Prevent the default behavior on click on an a tag 
  // (which is a hard page reload)
  e.preventDefault();
  // Instead change the url without reload
  history.pushState(null, '', href);
  showView();
});

// Show a view/"page"
function showView() {
  let route = location.pathname;
  // Find the corresponding menuItem index number to the href
  let index = menuItems.findIndex(x => '/' + kebabCase(x) === route);
  // If not found set the index to 0 (the first item)
  index = index < 0 ? 0 : index;
  // Get the content part corresponding to the menuItem
  let contentPart = content[index];
  // Replace the content in the main element
  $('main article').innerHTML = contentPart;
  // Add the css class active to the correct a tag in nav
  let navTags = [...document.querySelectorAll('nav a')];
  navTags.forEach(element => element.classList.remove('active'));
  navTags[index].classList.add('active');
}

// Listen to the back/forward buttons - change view based on url
window.addEventListener('popstate', () => showView());

// Show the first view after hard page load/reload
showView();