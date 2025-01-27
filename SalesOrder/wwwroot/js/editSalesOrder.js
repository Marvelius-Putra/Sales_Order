document.addEventListener('DOMContentLoaded', function () {
    flatpickr('.flatpickr');

    const salesOrderContainer = document.querySelector('#sales-order-container');
    const salesOrderId = salesOrderContainer.dataset.orderid;

    const editItemsBtn = document.querySelectorAll('.edit-item-btn');
    const saveItemBtns = document.querySelectorAll('.save-item-btn');
    const cancelItemBtns = document.querySelectorAll('.cancel-item-btn');
    const itemsTableBody = document.querySelector('.items-table tbody');
    const addItemBtn = document.getElementById('add-item-btn');
    const saveOrderBtn = document.getElementById('save-btn');

    let temporaryData = [];
    let deletedItems = [];
    let originalItemData = []; // Store original data for rows

    // Function to refresh row numbers
    function refreshRowNumbers() {
        const rows = itemsTableBody.querySelectorAll('tr');
        rows.forEach((row, index) => {
            row.querySelector('td:first-child').textContent = index + 1;
        });
    }

    // Function to create a new row in the table
    function createNewRow() {
        const newRowIndex = itemsTableBody.querySelectorAll('tr').length;

        const newRow = document.createElement('tr');
        newRow.innerHTML = `
            <td>${newRowIndex + 1}</td>
            <td>
                <button type="button" class="edit-item-btn" data-index="${newRowIndex}" style="display:none;">✎</button>
                <button type="button" class="delete-item-btn" data-index="${newRowIndex}" style="display:none;">🗑️</button>
                <button type="button" class="save-item-btn" data-index="${newRowIndex}">💾</button>
                <button type="button" class="cancel-item-btn" data-index="${newRowIndex}">❌</button>
            </td>
            <td><input type="text" class="form-control editable" placeholder="Item Name"></td>
            <td><input type="number" class="form-control editable inputQty" placeholder="Qty" min="1"></td>
            <td><input type="number" class="form-control editable inputPrice" placeholder="Price" step="0.01"></td>
            <td>0</td>
        `;
        itemsTableBody.appendChild(newRow);

        const inputs = newRow.querySelectorAll('input');
        inputs.forEach(input => {
            input.removeAttribute('readonly');
            input.classList.add('editable');
        });

        const saveBtn = newRow.querySelector('.save-item-btn');
        const cancelBtn = newRow.querySelector('.cancel-item-btn');
        saveBtn.style.display = 'inline-block';
        cancelBtn.style.display = 'inline-block';

        attachRowEventListeners(newRow, newRowIndex);
        storeOriginalData(newRow, newRowIndex); // Store original data for the new row
    }

    // Function to attach event listeners to row buttons
    function attachRowEventListeners(row, index) {
        const editBtn = row.querySelector('.edit-item-btn');
        const saveBtn = row.querySelector('.save-item-btn');
        const cancelBtn = row.querySelector('.cancel-item-btn');
        const deleteBtn = row.querySelector('.delete-item-btn');
        const inputs = row.querySelectorAll('input');

        editBtn.addEventListener('click', function () {
            inputs.forEach(input => {
                input.removeAttribute('readonly');
                input.classList.add('editable');
                input.style.border = '1px solid #ccc';
                input.style.background = '#ccc';
            });
            saveBtn.style.display = 'inline-block';
            cancelBtn.style.display = 'inline-block';
            editBtn.style.display = 'none';
            deleteBtn.style.display = 'none';
        });

        saveBtn.addEventListener('click', function () {
            const itemData = {
                itemName: inputs[0].value,
                quantity: parseFloat(inputs[1].value) || 0,
                price: parseFloat(inputs[2].value) || 0
            };
            itemData.total = itemData.quantity * itemData.price;
            temporaryData[index] = itemData; // Save data temporarily

            inputs.forEach(input => {
                input.setAttribute('readonly', 'true');
                input.classList.remove('editable');
                input.style.border = 'none';
                input.style.background = 'transparent';
            });
            row.querySelector('td:last-child').textContent = itemData.total.toLocaleString('id-ID');

            saveBtn.style.display = 'none';
            cancelBtn.style.display = 'none';
            editBtn.style.display = 'inline-block';
            deleteBtn.style.display = 'inline-block';
        });

        cancelBtn.addEventListener('click', function () {
            const savedData = originalItemData[index]; // Get original data

            // Restore the original values
            const inputs = row.querySelectorAll('input');
            inputs[0].value = savedData.itemName;  // Item Name
            inputs[1].value = savedData.quantity;  // Qty
            inputs[2].value = savedData.price;     // Price

            // Update the "Total" column
            row.querySelector('td:last-child').textContent = savedData.total.toLocaleString('id-ID');

            inputs.forEach(input => {
                input.setAttribute('readonly', 'true');
                input.classList.remove('editable');
                input.style.border = 'none';
                input.style.background = 'transparent';
            });

            saveBtn.style.display = 'none';
            cancelBtn.style.display = 'none';
            editBtn.style.display = 'inline-block';
            deleteBtn.style.display = 'inline-block';
        });

        deleteBtn.addEventListener('click', function () {
            const confirmDelete = window.confirm('Do you really want to delete this item?');
            if (confirmDelete) {
                const rowToDelete = itemsTableBody.querySelectorAll('tr')[index];
                if (rowToDelete) {
                    const inputs = rowToDelete.querySelectorAll('input');
                    const itemId = rowToDelete.dataset.itemid || '';
                    const deletedItem = {
                        id: itemId,
                        itemName: inputs[0].value,
                        quantity: parseFloat(inputs[1].value) || 0,
                        price: parseFloat(inputs[2].value) || 0,
                        total: (parseFloat(inputs[1].value) || 0) * (parseFloat(inputs[2].value) || 0)
                    };

                    deletedItems.push(deletedItem);

                    rowToDelete.remove();

                    temporaryData.splice(index, 1);

                    refreshRowNumbers();
                }
            }
        });
    }

    // Function to store the original data of a row
    function storeOriginalData(row, index) {
        const inputs = row.querySelectorAll('input');
        const originalData = {
            itemName: inputs[0].value,
            quantity: parseFloat(inputs[1].value) || 0,
            price: parseFloat(inputs[2].value) || 0,
            total: (parseFloat(inputs[1].value) || 0) * (parseFloat(inputs[2].value) || 0)
        };
        originalItemData[index] = originalData; // Store original data
    }

    // Event listener for adding a new row
    addItemBtn.addEventListener('click', function (e) {
        e.preventDefault();
        createNewRow();
        refreshRowNumbers();
    });

    // Attach event listeners to existing rows
    itemsTableBody.querySelectorAll('tr').forEach((row, index) => {
        attachRowEventListeners(row, index);
        storeOriginalData(row, index); // Store original data for existing rows
    });

    // Save entire order
    saveOrderBtn.addEventListener('click', function (e) {
        e.preventDefault();

        const salesOrder = {
            id: salesOrderId,
            orderNumber: document.getElementById('salesOrderNumber').value,
            orderDate: document.getElementById('orderDate').value,
            customerId: document.getElementById('customer').value,
            address: document.getElementById('address').value,
            items: []
        };

        itemsTableBody.querySelectorAll('tr').forEach(row => {
            const inputs = row.querySelectorAll('input');
            const itemId = row.dataset.itemid;
            const item = {
                id: itemId,
                itemName: inputs[0].value,
                quantity: parseFloat(inputs[1].value) || 0,
                price: parseFloat(inputs[2].value) || 0
            };
            if (item.itemName && item.quantity > 0 && item.price > 0) {
                salesOrder.items.push(item);
            }
        });

        fetch('/RequestOrder/DeleteItems', {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(deletedItems)
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => { throw new Error(text); });
                }
                return response.json();
            })
            .then(data => {
                console.log('Deleted items:', data);
            })
            .catch((error) => {
                console.error('Error deleting items:', error);
            });

        fetch('/RequestOrder/UpdateSales', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(salesOrder)
        })
            .then(response => {
                if (response.ok) {
                    console.log('Items saved successfully!');
                    alert('Order saved successfully!');
                    window.location.href = '/';
                } else {
                    return response.text().then(text => { throw new Error(text); });
                }
            })
            .catch(error => {
                console.error('Error saving items:', error);
            });
    });
});
