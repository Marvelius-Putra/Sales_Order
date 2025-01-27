let itemCount = 0;
let temporaryItems = [];


document.getElementById("add-item-btn").addEventListener("click", function () {
    const tableBody = document.querySelector("#items-table tbody");


    const placeholderRow = document.querySelector(".placeholder-row");
    if (placeholderRow) {
        placeholderRow.remove();
    }
    itemCount++;

    const row = document.createElement("tr");
    row.innerHTML = `
        <td>${itemCount}</td>
        <td><button class="save-btn">💾</button></td>
        <td><input type="text" class="item-name" placeholder="Enter Item Name"></td>
        <td><input type="number" class="qty-input" placeholder="0" min="1" oninput="calculateRowTotal(this)"></td>
        <td><input type="number" class="price-input" placeholder="0.00" step="0.01" oninput="calculateRowTotal(this)"></td>
        <td><span class="total">0.00</span></td>
    `;
    tableBody.appendChild(row);

    const saveButton = row.querySelector(".save-btn");
    saveButton.addEventListener("click", () => saveRow(row));
});

// stored icon saved database to temporary
function saveRow(row) {
    const itemNameInput = row.querySelector(".item-name");
    const qtyInput = row.querySelector(".qty-input");
    const priceInput = row.querySelector(".price-input");

    // Input validation
    if (!itemNameInput.value.trim() || !qtyInput.value || !priceInput.value) {
        alert("Please fill in all fields before saving!");
        return;
    }

    // Store to temporary data in array
    const itemData = {
        itemName: itemNameInput.value.trim(),
        quantity: parseInt(qtyInput.value, 10),
        price: parseFloat(priceInput.value),
    };

    const rowIndex = row.dataset.index;

    if (rowIndex !== undefined) {
        temporaryItems[rowIndex] = itemData;
    } else {
        temporaryItems.push(itemData);
        row.dataset.index = temporaryItems.length - 1; 
    }

    // Disable inputs dan hide saved button
    itemNameInput.setAttribute("disabled", true);
    qtyInput.setAttribute("disabled", true);
    priceInput.setAttribute("disabled", true);
    row.querySelector(".save-btn").style.display = "none";

    // Update summary
    updateSummary();
    alert("Item disimpan sementara.");
}

// Function to count rows
function calculateRowTotal(input) {
    const row = input.closest("tr");
    const qtyInput = row.querySelector(".qty-input").value || 0;
    const priceInput = row.querySelector(".price-input").value || 0;

    const totalCell = row.querySelector(".total");
    const rowTotal = parseFloat(qtyInput) * parseFloat(priceInput);

    // Update total value
    totalCell.textContent = rowTotal.toLocaleString('id-ID');

    // Update summary
    updateSummary();
}

// Function to update total and amount of items
function updateSummary() {
    const rows = document.querySelectorAll("#items-table tbody tr");
    let totalItems = 0;
    let totalAmount = 0;

    rows.forEach((row) => {
        const qty = parseFloat(row.querySelector(".qty-input")?.value) || 0;
        const price = parseFloat(row.querySelector(".price-input")?.value) || 0;

        totalItems += qty;
        totalAmount += qty * price;
    });

    const totalItemElement = document.getElementById("total-item");
    const totalAmountElement = document.getElementById("total-amount");

    if (totalItemElement) {
        totalItemElement.textContent = totalItems.toLocaleString('id-ID');;
    }

    if (totalAmountElement) {
        totalAmountElement.textContent = totalAmount.toLocaleString("id-ID", {
            style: "currency",
            currency: "IDR"
        });
    }
}


document.getElementById("save-btn").addEventListener("click", function () {
    const rows = document.querySelectorAll("#items-table tbody tr");

    rows.forEach((row) => {
        const itemNameInput = row.querySelector(".item-name");
        const qtyInput = row.querySelector(".qty-input");
        const priceInput = row.querySelector(".price-input");

        if (itemNameInput && qtyInput && priceInput) {
            if (!itemNameInput.disabled) {
                const itemData = {
                    itemName: itemNameInput.value.trim(),
                    quantity: parseInt(qtyInput.value, 10) || 0,
                    price: parseFloat(priceInput.value) || 0,
                };

                // Insert data validation
                if (itemData.itemName && itemData.quantity > 0 && itemData.price > 0) {
                    temporaryItems.push(itemData); 
                    hasData = true;
                }
            }
        }
    });

    // Validasi if there is data
    if (temporaryItems.length === 0) {
        alert("Tidak ada item untuk disimpan!");
        return;
    }

    const salesOrderData = {
        orderNumber: document.getElementById("sales-order-number").value,
        orderDate: document.getElementById("order-date").value,
        customerId: document.getElementById("customer").value,
        address: document.getElementById("address").value,
        items: temporaryItems
    };

    fetch("/RequestOrder/SaveOrder", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(salesOrderData)
    })
        .then((response) => {
            if (!response.ok) {
                throw new Error("Network response was not ok " + response.statusText);
            }
            return response.json();
        })
        .then((data) => {
            if (data.success) {
                alert("Order berhasil disimpan!");
                window.location.href = '/';
            } else {
                alert("Gagal menyimpan order: " + data.message);
            }
        })
        .catch((error) => {
            console.error("Error:", error);
            alert("Terjadi kesalahan saat menyimpan order. Pastikan seluruh data terisi");
        });
});

document.addEventListener("DOMContentLoaded", function () {
    flatpickr("#order-date", {
        dateFormat: "Y-m-d",
        altInput: true, 
        altFormat: "F j, Y", 
        allowInput: true
    });
});

document.getElementById('close-btn').addEventListener('click', function () {
    window.location.href = '/';
});




