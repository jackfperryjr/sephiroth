
// Loading characters once page is ready
$(document).ready(function () {
    getData();
});

// Using Api to get characters
const uri = '/api/character';
function getData() {
    $.ajax({
        type: 'GET',
        url: uri,
        success: function (data) {
            $('#characters').empty();
            getCount(data.length);
            $.each(data, function (key, character) {
                $('<tr><td>' + character.name + '</td>' +
                    '<td>' + character.origin + '</td>' +
                    '</tr>').appendTo($('#characters'));
            });

            characters = data;
        }
    });
}

// Counting the characters
let characters = null;
function getCount(data) {
    const el = $('#counter');
    let name = 'character';
    if (data) {
        if (data > 1) {
            name = 'characters';
        }
        el.text(data + ' ' + name);
    } else {
        el.html('No ' + name);
    }
}

// Finding characters upon search
$('#search').keyup(function() {
    let searchField = $(this).val();
    if(searchField === '') {
        $('#result').html('');
        return;
    }
    let regex = new RegExp(searchField, "i");
    let output = '';
    let count = 1;

        // Display character format
        $.each(characters, function(key, val) {
            if ((val.name.search(regex) != -1)) {
                output += '<div class="card col-md-12">';
                output += '<span>&#123;</span><code>';
                output += '"name":"' + val.name.toLowerCase() + '",' + '<br/>';
                output += '"age":"' + val.age.toLowerCase() + '",' + '<br/>';
                output += '"race":"' + val.race.toLowerCase() + '",' + '<br/>';
                output += '"job":"' + val.job.toLowerCase() + '",' + '<br/>';
                output += '"height":"' + val.height.toLowerCase() + '",' + '<br/>';
                output += '"weight:"' + val.weight.toLowerCase() + '",' + '<br/>';
                output += '"origin":"' + val.origin.toLowerCase() + '",' + '<br/>';
                output += '"description":"' + val.description.toLowerCase() + '"' + '<br/>';
                output += '</code><span>&#125;</span></div><br/>';
                output += '</div>';
                if(count%2 == 0) { 
                    output += '</div>'
                }
                count++;
            }
        });
    $('#result').html(output);
});

// Click toggle for full list of characters
$('#list-button').click(function() {
    $('#full-list').fadeToggle();
});

// Displaying current year in footer
let year = new Date().getFullYear();
$('#footer').html("&copy; " + year + " jack f. perry, jr.");