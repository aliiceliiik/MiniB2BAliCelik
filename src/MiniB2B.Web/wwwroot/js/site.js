const App = (() => {
    const tokenMeta = document.querySelector('meta[name="request-verification-token"]');
    const currencyFormatter = new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' });

    function extractError(data) {
        if (data?.message) return data.message;
        if (data?.errors) return Object.values(data.errors).flat()[0];
        return 'Beklenmeyen bir hata oluştu.';
    }

    async function api(url, method = 'GET', body) {
        const response = await fetch(url, {
            method,
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': tokenMeta?.content ?? ''
            },
            body: body ? JSON.stringify(body) : undefined
        });

        if (response.status === 401) {
            const returnUrl = encodeURIComponent(location.pathname + location.search);
            window.location.href = `/Account/Login?returnUrl=${returnUrl}`;
            throw new Error('Oturumunuz sona erdi.');
        }

        const data = await response.json().catch(() => null);

        if (!response.ok) {
            throw new Error(extractError(data));
        }

        return data;
    }

    function toast(message, type = 'success') {
        const container = document.getElementById('toast-container');
        const element = document.createElement('div');
        element.className = `alert alert-${type} shadow-sm mb-2`;
        element.textContent = message;
        container.appendChild(element);
        setTimeout(() => element.remove(), 3500);
    }

    function updateCartBadge(count) {
        const badge = document.getElementById('cart-count-badge');
        if (badge) badge.textContent = count;
    }

    return {
        api,
        toast,
        updateCartBadge,
        formatCurrency: value => currencyFormatter.format(value)
    };
})();