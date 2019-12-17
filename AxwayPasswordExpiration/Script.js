var table = document.querySelector('#accountListForm > table > tbody > tr:nth-child(3) > td > table > tbody');
console.log('Username, ' + 'DaysUntilExpire, ' + 'LastPasswordChange, ' + 'PasswordExpireInterval');

for (var i = 1, row; (row = table.rows[i]); i++) {
    //iterate through rows

    var loginName = row.cells[2].innerText;

    if (!loginName.toUpperCase().includes('MASTER')) {
        const Http1 = new XMLHttpRequest();
	const url1 = 'https://prd-axwftp0010:444/api/v1.4/accounts/' + loginName + '/users';
	Http1.open('GET', url1);
	Http1.send();

	Http1.onreadystatechange = (e) => {
            var response = JSON.parse(Http1.responseText);
            var username = response.users[0].passwordCredentials.username;
	    var lastPasswordChange = response.users[0].passwordCredentials.lastPasswordChange.substring(5, 16);
	    var passwordExpiryInterval = response.users[0].passwordCredentials.passwordExpiryInterval;
	    var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
	    var firstDate = new Date(lastPasswordChange);
	    var secondDate = new Date();
	    var daysSinceChange = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / oneDay));
	    var daysUntilExpire = 180 - daysSinceChange;
	    console.log(', ' + username + ', ' + daysUntilExpire + ', ' + lastPasswordChange.replace(',', '') +
                ',  ' + passwordExpiryInterval + ', ');
	};
    }
}
