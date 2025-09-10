const routes = new Object({
    0: '/people',
    1: '/people/new-person',
    2: '/countries/upload-excel',
});

let buttonMenu = document.querySelector('#buttonMenu');
let menuItems = document.querySelector('#menuItems');
let options = document.querySelectorAll('.options');

buttonMenu.addEventListener('click', () => {
    menuItems.classList.toggle('hidden');
});

for (let i = 0; i < Object.keys(routes).length; i++) {
    console.log(document.location.pathname)
    if (document.location.pathname == routes[i]) {
        options[i].classList.add('menu-options-selected');
    } else {
        options[i].classList.remove('menu-options-selected');
    }
}

document.addEventListener('click', () => {
    if (!menuItems.classList.contains('hidden') && (document.activeElement.id != 'buttonMenu' && document.activeElement.id != 'menuItems')) {
        menuItems.classList.add('hidden');
    }
});