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