window.addEventListener('load', function () {
  var urlParams = new URLSearchParams(window.location.search);
  var username = urlParams.get('username');

  fetch('users.json')
    .then(response => response.json())
    .then(data => {
      var user = data.find(function (user) {
        return user.username === username;
      });

      document.getElementById('username').value = user.username;
      document.getElementById('firstname').value = user.firstname;
      document.getElementById('lastname').value = user.lastname;
      document.getElementById('age').value = user.age;
      document.getElementById('email').value = user.email;
      document.getElementById('phonenumber').value = user.phonenumber;
    })
    .catch(error => console.error(error));

  document.getElementById('edit-user-form').addEventListener('submit', function (event) {
    event.preventDefault();

    var formData = new FormData(this);
    var updatedUser = {};
    for (var pair of formData.entries()) {
      updatedUser[pair[0]] = pair[1];
    }

    fetch('updateuser.php', {
      method: 'POST',
      body: JSON.stringify(updatedUser),
      headers: {
        'Content-Type': 'application/json'
      }
    })
      .then(response => response.json())
      .then(data => {
        console.log('User updated:', data);
        alert('User updated successfully!');
        window.location.href = 'index.html';
      })
      .catch(error => console.error(error));
  });
});
