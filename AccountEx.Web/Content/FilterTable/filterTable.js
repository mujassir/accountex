
function filterTable(term, table) {
    dehighlight(table);
    var terms = term.value.toLowerCase().split(" ");

    for (var r = 1; r < table.rows.length; r++) {
        var display = '';
        for (var i = 0; i < terms.length; i++) {
            var tdText = table.rows[r].innerHTML.replace(/<[^>]+>/g, "").toLowerCase();
            //var inputValue = $(table.rows[r]).find("input").val();
            if ($(table.rows[r]).attr("data-header-row"))
                continue;
            if (tdText.indexOf(terms[i]) < 0) {
                display = 'none';
            } else {
                if (terms[i].length) highlight(terms[i], table.rows[r]);
            }
            table.rows[r].style.display = display;
        }
    }
    $(table).find("tr[data-header-row='true']").each(function () {
        var tr = $(this);
        var parent = $(tr).attr("data-parent");
        var childRows = $(table).find("[data-parent='" + parent + "']:not([data-header-row='true'])");

        if (!Common.isNullOrWhiteSpace(parent)) {
            var numOfVisibleRows = $(childRows).filter(function () {
                return $(this).css('display') !== 'none';
            }).length;
            if (numOfVisibleRows >= 1) {
                $(tr).css("display", "")
            }
            else {
                $(tr).css("display", 'none')
            }
        }
    });
}



function dehighlight(container) {
    for (var i = 0; i < container.childNodes.length; i++) {
        var node = container.childNodes[i];

        if (node.attributes && node.attributes['class']
			&& node.attributes['class'].value == 'table-search-highlighted') {
            node.parentNode.parentNode.replaceChild(
					document.createTextNode(
						node.parentNode.innerHTML.replace(/<[^>]+>/g, "")),
					node.parentNode);
            // Stop here and process next parent
            return;
        } else if (node.nodeType != 3) {
            // Keep going onto other elements
            dehighlight(node);
        }
    }
}


function highlight(term, container) {
    for (var i = 0; i < container.childNodes.length; i++) {
        var node = container.childNodes[i];

        if (node.nodeType == 3) {
            // Text node
            var data = node.data;
            var data_low = data.toLowerCase();
            if (data_low.indexOf(term) >= 0) {
                //term found!
                var new_node = document.createElement('span');

                node.parentNode.replaceChild(new_node, node);

                var result;
                while ((result = data_low.indexOf(term)) != -1) {
                    new_node.appendChild(document.createTextNode(
								data.substr(0, result)));
                    new_node.appendChild(create_node(
								document.createTextNode(data.substr(
										result, term.length))));
                    data = data.substr(result + term.length);
                    data_low = data_low.substr(result + term.length);
                }
                new_node.appendChild(document.createTextNode(data));
            }
        } else {
            // Keep going onto other elements
            highlight(term, node);
        }
    }
}

function create_node(child) {
    var node = document.createElement('span');
    node.setAttribute('class', 'table-search-highlighted');
    node.attributes['class'].value = 'table-search-highlighted';
    node.appendChild(child);
    return node;
}




tables = document.getElementsByTagName('table');
for (var t = 0; t < tables.length; t++) {
    element = tables[t];

    if (element.attributes['class']
		&& element.attributes['class'].value == 'filterable') {

        /* Here is dynamically created a form */
        var form = document.createElement('form');
        form.setAttribute('class', 'filter');
        // For ie...
        form.attributes['class'].value = 'filter';
        var input = document.createElement('input');
        input.onkeyup = function () {
            filterTable(input, element);
        }
        form.appendChild(input);
        element.parentNode.insertBefore(form, element);
    }
}

