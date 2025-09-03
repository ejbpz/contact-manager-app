let deleteModal = document.querySelector('#deleteModal');
let deleteButtons = document.querySelectorAll('#deleteButton');

deleteButtons.forEach(button => button.addEventListener('click', async () => {
    deleteModal.classList.remove('hidden');
    deleteModal.classList.add('block');
    let personId = button.dataset.id;

    await fetch(`/people/delete-view/${personId}`, { method: "GET" })
        .then(res => res.text())
        .then(html => deleteModal.innerHTML = html)

    let closeModal = document.querySelector('#closeModal');
    closeModal.addEventListener('click', () => {
        deleteModal.classList.add('hidden');
        deleteModal.classList.remove('block');
    });
}));
