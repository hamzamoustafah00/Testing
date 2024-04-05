
import './stringExtras.js';
import { $ } from './jQueryish.js';
import { fetchEasy } from './fetchEasy.js';

// Read content
const htmlSkeleton =
  await fetchEasy('/content/bodySkeleton.html');
const content =
  (await fetchEasy('/content/_content.md')).splitHtml(1);

// Extract the h1:s as menu items
const menuItems = content.map(x => x.slice(4).split('<')[0]);

// Add initial html for the site
$('body').html(htmlSkeleton.revive({
  menu:
    menuItems.map((item, index) => /*html*/`
      <a href="/${index === 0 ? '' : item.kebabCase()}">${item}</a>
    `).join('')
}));

// When we click somewhere - check if the click
// is on an a tag with an internal link
$('a[href^="/"]').click((el, event) => {
  // Prevent the default behavior on click on an a tag 
  // (which is a hard page reload)
  event.preventDefault();
  // Instead change the url without reload
  history.pushState(null, '', el.attr('href'));
  showView();
});

// Show a view/"page"
function showView() {
  let route = location.pathname;
  // Find the corresponding menuItem index number to the href
  let index = menuItems.findIndex(x => '/' + x.kebabCase() === route);
  // If not found set the index to 0 (the first item)
  index = index < 0 ? 0 : index;
  // Replace the content in the main element
  $('main article').html(content[index]);
  // Add the css class 'active' to the active menu item
  $('nav a').removeClass('active').eq(index).addClass('active');
}

// Listen to the back/forward buttons - change view based on url
window.addEventListener('popstate', () => showView());

// Show the first view after hard page load/reload
showView();