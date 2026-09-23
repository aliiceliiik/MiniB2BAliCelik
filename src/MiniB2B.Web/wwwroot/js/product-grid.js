document.addEventListener('click', async (event) => {
    const button = event.target.closest('.js-add-to-cart');
    if (!button) return;

    const qtyInput = button.closest('.input-group').querySelector('.js-qty');
    const quantity = parseInt(qtyInput.value, 10);

    if (!Number.isInteger(quantity) || quantity < 1) {
        App.toast('Lütfen geçerli bir adet giriniz.', 'warning');
        qtyInput.focus();
        return;
    }

    button.disabled = true;

    try {
        const cart = await App.api('/api/cart/items', 'POST', {
            productId: Number(button.dataset.productId),
            quantity
        });

        App.updateCartBadge(cart.itemCount);
        App.toast('Ürün sepete eklendi.');
        qtyInput.value = 1;
    } catch (error) {
        App.toast(error.message, 'danger');
    } finally {
        button.disabled = false;
    }
});

const productModal = document.getElementById('productModal');

if (productModal) {
    const modal = new bootstrap.Modal(productModal);
    const body = document.getElementById('productModalBody');
    const title = document.getElementById('productModalTitle');

    const stockLabels = {
        Available: ['Var', 'bg-success'],
        Critical: ['Kritik', 'bg-warning text-dark'],
        OutOfStock: ['Yok', 'bg-danger']
    };

    function renderDetail(p) {
        const [stockText, stockClass] = stockLabels[p.stockStatus] ?? ['-', 'bg-secondary'];
        const rows = [
            ['Ürün Kodu', p.productCode],
            ['Marka', p.brand],
            ['Kategori', p.categoryName],
            ['Üretici Kodu', p.manufacturerCode],
            ['Özel Kod 1', p.specialCode1],
            ['Özel Kod 2', p.specialCode2]
        ];

        body.textContent = '';

        const wrapper = document.createElement('div');
        wrapper.className = 'row g-3';
        wrapper.innerHTML = `
            <div class="col-md-4">
                ${p.imageUrl ? `<img src="${encodeURI(p.imageUrl)}" class="img-fluid rounded" alt="" />`
                : '<div class="text-muted small">Görsel yok</div>'}
            </div>
            <div class="col-md-8">
                <table class="table table-sm mb-2"><tbody></tbody></table>
                <div class="fs-5 fw-semibold">${App.formatCurrency(p.price)}</div>
                <div class="mt-2">Stok: <span class="badge ${stockClass}">${stockText}</span></div>
                <div class="mt-3 text-muted small js-description"></div>
            </div>`;

        const tbody = wrapper.querySelector('tbody');

        for (const [label, value] of rows) {
            if (!value) continue;
            const tr = document.createElement('tr');
            const th = document.createElement('th');
            th.className = 'text-muted fw-normal';
            th.style.width = '140px';
            th.textContent = label;
            const td = document.createElement('td');
            td.textContent = value;
            tr.append(th, td);
            tbody.appendChild(tr);
        }

        wrapper.querySelector('.js-description').textContent = p.description ?? '';
        body.appendChild(wrapper);
        title.textContent = p.name;
    }

    document.addEventListener('click', async (event) => {
        if (event.target.closest('.js-add-to-cart, .js-qty, a, input, button')) return;

        const row = event.target.closest('.js-product-row');
        if (!row) return;

        body.innerHTML = '<div class="text-center text-muted py-4">Yükleniyor...</div>';
        title.textContent = 'Ürün Detayı';
        modal.show();

        try {
            renderDetail(await App.api(`/api/products/${row.dataset.productId}`));
        } catch (error) {
            body.textContent = error.message;
        }
    });
}