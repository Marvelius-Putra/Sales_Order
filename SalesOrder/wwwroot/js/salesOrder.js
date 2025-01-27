//seacrh button
document.getElementById("search-btn").addEventListener("click", function () {
    loadData(1); 
});

// add button method
document.getElementById('add-btn').addEventListener('click', function () {
    window.location.href = '/SalesOrder/Create'; // Update path ke halaman detail input
});

// export to excel method
document.getElementById("export-btn").addEventListener("click", function () {
    exportToExcel();
});

// edit button method
document.addEventListener("DOMContentLoaded", function () {
    document.querySelector("#salesOrderTable").addEventListener("click", function (event) {
        if (event.target.classList.contains("edit-btn")) {
            const orderId = event.target.getAttribute("data-id"); 
            goToEditPage(orderId); 
        }
    });
});

// Function to direct to edit page
function goToEditPage(id) {
    window.location.href = `/SalesOrder/Edit?id=${id}`;
}

// function to export excel
function exportToExcel() {
    const table = document.getElementById('salesOrderTable');
    const rows = table.querySelectorAll('tbody tr');
    const data = [['No', 'Sales Order', 'Order Date', 'Customer']]; 

    rows.forEach(row => {
        const cells = row.querySelectorAll('td');
        if (cells.length > 0) {
            data.push([
                cells[0].textContent.trim(), 
                cells[2].textContent.trim(), 
                cells[3].textContent.trim(), 
                cells[4].textContent.trim()  
            ]);
        }
    });

    const worksheet = XLSX.utils.aoa_to_sheet(data);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'SalesOrders');
    XLSX.writeFile(workbook, 'SalesOrders.xlsx');
}

// Function for pagination
function loadData(pageNumber = 1) {
    const pageSize = 5;
    const keywords = document.getElementById("keywords").value;
    const orderDate = document.getElementById("order-date").value;

    fetch(`/Home/GetPagedOrders?pageNumber=${pageNumber}&pageSize=${pageSize}&keywords=${encodeURIComponent(keywords)}&orderDate=${encodeURIComponent(orderDate)}`)
        .then((response) => response.json())
        .then((result) => {
            renderTable(result.data);
            renderPagination(result.totalPages, result.currentPage);
        })
        .catch((error) => console.error("Error loading data:", error));
}

//function to display pagination
function renderPagination(totalPages, currentPage) {
    const pagination = document.querySelector(".pagination");
    pagination.innerHTML = "";

    pagination.innerHTML += `
        <button class="page-btn" ${currentPage === 1 ? "disabled" : ""} data-page="${currentPage - 1}">
            «
        </button>
    `;

    for (let i = 1; i <= totalPages; i++) {
        pagination.innerHTML += `
            <button class="page-btn ${i === currentPage ? "active" : ""}" data-page="${i}">
                ${i}
            </button>
        `;
    }

    pagination.innerHTML += `
        <button class="page-btn" ${currentPage === totalPages ? "disabled" : ""} data-page="${currentPage + 1}">
            »
        </button>
    `;

    document.querySelectorAll(".page-btn").forEach((button) => {
        button.addEventListener("click", () => {
            const page = parseInt(button.dataset.page);
            loadData(page);
        });
    });
}

// Function to display table
function renderTable(data) {
    const tbody = document.querySelector("#salesOrderTable tbody");
    tbody.innerHTML = "";

    if (data.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5" style="text-align: center;">No results found.</td>
            </tr>
        `;
        return;
    }

    data.forEach((order, index) => {
        const formattedDate = new Intl.DateTimeFormat('en-GB').format(new Date(order.orderDate));
        tbody.innerHTML += `
        <tr>
            <td>${order.id}</td>
            <td>
                <button class="edit-btn" data-id="${order.id}">✎</button>
                <button class="delete-btn" data-id="${order.id}">🗑️</button>
            </td>
            <td>${order.orderNumber}</td>
            <td>${formattedDate}</td> <!-- Format dd/MM/yyyy -->
            <td>${order.customer}</td>
        </tr>
    `;
    });

}

// delete method
document.querySelector("#salesOrderTable").addEventListener("click", function (event) {
    if (event.target.classList.contains("delete-btn")) {
        const orderId = event.target.getAttribute("data-id");

        if (confirm("Are you sure you want to delete this Sales Order?")) {
            deleteSalesOrder(orderId);
        }
    }
});


// function to delete sales order
function deleteSalesOrder(orderId) {
    fetch(`/RequestOrder/DeleteOrder?id=${orderId}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json"
        }
    })
        .then(response => {
            if (response.ok) {
                alert("Sales Order deleted successfully.");
                loadData(); // Refresh tabel setelah penghapusan
            } else {
                return response.json().then(data => {
                    throw new Error(data.message || "Failed to delete Sales Order.");
                });
            }
        })
        .catch(error => {
            console.error("Error deleting Sales Order:", error);
            alert(error.message);
        });
}

// pick date method
document.addEventListener("DOMContentLoaded", function () {
    flatpickr("#order-date", {
        dateFormat: "d/m/Y",
        allowInput: true,
        altInput: false,
        defaultDate: null
    });

    loadData(1); 
});

// Enter = search button
document.addEventListener('keydown', function (event) {
    if (event.key === 'Enter') { 
        event.preventDefault(); 
        document.getElementById("search-btn").click();
    }
});