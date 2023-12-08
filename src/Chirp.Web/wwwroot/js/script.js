//the url we're on
var pageName = window.location.pathname;

document.addEventListener('DOMContentLoaded', function() {
    var followForm = document.getElementById('followForm');
    var unfollowForm = document.getElementById('unfollowForm');

    if (followForm) {
        followForm.addEventListener('submit', function(e) {
            e.preventDefault();
            followOrUnfollow('Follow', e.target);
        });
    }
    
    if (unfollowForm) {
        unfollowForm.addEventListener('submit', function(e) {
            e.preventDefault();
            followOrUnfollow('Unfollow', e.target);
        });
    }
});

function followOrUnfollow(action, form) {
    fetch(`/${action}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: JSON.stringify({ author: form.querySelector("input[name='author']").value })
    })
    .then(response => response.json())
    .then(data => {
        if (data.status === 'success') {
            const newAction = action === 'Follow' ? 'Unfollow' : 'Follow';
            form.querySelector('button').textContent = newAction;

            // After a successful operation, replace the form's event listener to handle the next action
            form.removeEventListener('submit');
            form.addEventListener('submit', function(e) {
                e.preventDefault();
                followOrUnfollow(newAction, form);
            });
        } else {
            console.error('Error:', data.message);
        }
    });
}

//error checking
console.log(JSON.stringify({ author: 'Author Name' }));
console.log(pageName);

//fetch stuff
fetch(`${pageName}?handler=Follow`, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest' // This header tells the server that this is an AJAX request.
    },
    body: JSON.stringify({ author: 'Author Name' })
}).then(response => response.json()).then(response => {
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    } else {
        return response.json();
    }
}).then(data => {
    if (data.status === 'success') {
        form.querySelector('button').textContent = 'Follow';
    } else {
        console.error('Error:', data.message);
    }
}).catch(e => {
    console.log('There was a problem with the fetch operation: ' + e.message);
});

fetch(`${pageName}?handler=Unfollow`, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'X-Requested-With': 'XMLHttpRequest' // This header tells the server that this is an AJAX request.
    },
    body: JSON.stringify({ author: 'Author Name' })
}).then(response => response.json()).then(response => {
    if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
    } else {
        return response.json();
    }
}).then(data => {
    if (data.status === 'success') {
        form.querySelector('button').textContent = 'Follow';
    } else {
        console.error('Error:', data.message);
    }
}).catch(e => {
    console.log('There was a problem with the fetch operation: ' + e.message);
});