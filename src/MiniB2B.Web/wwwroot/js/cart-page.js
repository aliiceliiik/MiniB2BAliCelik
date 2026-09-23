(() => {
    const page = document.getElementById('cart-page');
    if (!page) return;

    function applyCart(cart) {
        if (cart.itemCount === 0) {
            window.location.reload();
            return;
        }

        for (const item of cart.items) {
            const row = page.querySelector(`tr[data-product-id="${item.productId}"]`);
            if (!row) continue;

            row.dataset.availableStock = item.availableStock;
            row.querySelector('.js-line-total').textContent = App.formatCurrency(item.lineTotal);
            row.querySelector('.js-available').textContent = item.availableStock;
            row.querySelector('.js-stock-warning').classList.toggle('d-none', item.hasSufficientStock);
        }

        document.getElementById('cart-total').textContent =
            App.formatCurrency(cart.totalAmount);

        document.getElementById('checkout-btn').disabled = !cart.canCheckout;

        document.getElementById('checkout-warning').classList.toggle(
            'd-none',
            cart.canCheckout
        );

        App.updateCartBadge(cart.itemCount);
    }

    async function refreshCart() {
        try {
            applyCart(await App.api('/api/cart'));
        } catch {
            window.location.reload();
        }
    }

    page.addEventListener('change', async (event) => {
        const input = event.target.closest('.js-cart-qty');
        if (!input) return;

        const row = input.closest('tr');
        const quantity = parseInt(input.value, 10);
        const availableStock = parseInt(row.dataset.availableStock, 10);

        if (!Number.isInteger(quantity) || quantity < 1) {
            App.toast('Adet en az 1 olmalıdır.', 'warning');
            input.value = input.defaultValue;
            return;
        }

        if (quantity > availableStock) {
            App.toast(
                `${row.dataset.productName} için yeterli stok bulunmamaktadır. Mevcut stok: ${availableStock}.`,
                'warning'
            );

            input.value = input.defaultValue;
            return;
        }

        try {
            const cart = await App.api(
                `/api/cart/items/${row.dataset.productId}`,
                'PUT',
                { quantity }
            );

            input.defaultValue = quantity;
            applyCart(cart);
        } catch (error) {
            App.toast(error.message, 'danger');
            input.value = input.defaultValue;
        }
    });

    page.addEventListener('click', async (event) => {
        const button = event.target.closest('.js-remove');
        if (!button) return;

        if (!confirm('Ürünü sepetten çıkarmak istiyor musunuz?')) return;

        const row = button.closest('tr');

        try {
            const cart = await App.api(
                `/api/cart/items/${row.dataset.productId}`,
                'DELETE'
            );

            row.remove();
            applyCart(cart);

            App.toast('Ürün sepetten çıkarıldı.');
        } catch (error) {
            App.toast(error.message, 'danger');
        }
    });

    document.getElementById('checkout-btn').addEventListener('click', async (event) => {
        const button = event.currentTarget;

        if (!confirm('Siparişinizi oluşturmak istiyor musunuz?')) return;

        button.disabled = true;

        try {
            const order = await App.api('/api/orders', 'POST');
            window.location.href = `/Orders/Details/${order.orderId}?created=true`;
        } catch (error) {
            App.toast(error.message, 'danger');
            await refreshCart();
        }
    });
})();
