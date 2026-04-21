document.addEventListener("DOMContentLoaded", function() {
    
    /* ==========================================================================
       ACESSIBILIDADE (WCAG): PREFERÊNCIA DE MOVIMENTO
       Verifica se o usuário configurou o sistema operacional para reduzir movimentos.
       Isso previne gatilhos de labirintite ou convulsões.
       ========================================================================== */
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    
    // Animações de Entrada Suaves (Intersection Observer otimiza a performance)
    if (!prefersReducedMotion) {
        const observerOptions = { 
            root: null, 
            rootMargin: '0px', 
            threshold: 0.1 
        };
        
        const observer = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target); // Para de observar após animar
                }
            });
        }, observerOptions);

        document.querySelectorAll('.fade-in-element').forEach(el => observer.observe(el));
    } else {
        // Fallback de Acessibilidade: Exibe tudo imediatamente sem animação
        document.querySelectorAll('.fade-in-element').forEach(el => el.classList.add('is-visible'));
    }

    /* ==========================================================================
       USABILIDADE (NIELSEN): FEEDBACK E PREVENÇÃO DE ERROS
       Lida com os formulários de Cadastro e Login dinamicamente.
       ========================================================================== */
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', function() {
            
            // Verifica a validade nativa do HTML5 antes de processar
            if (form.checkValidity()) {
                const btnSubmit = form.querySelector('button[type="submit"]');
                
                if (btnSubmit) {
                    const btnText = btnSubmit.querySelector('.btn-text');
                    
                    // Heurística 5 (Prevenção de Erros): Desabilita o clique para evitar duplo envio
                    btnSubmit.style.opacity = '0.85';
                    btnSubmit.style.cursor = 'wait';
                    btnSubmit.style.pointerEvents = 'none'; 
                    
                    // Heurística 1 (Visibilidade do Status) e Heurística 2 (Mundo Real): 
                    // Muda o texto com base no ID do formulário atual
                    if (btnText) {
                        const isLogin = form.id === 'form-login';
                        btnText.textContent = isLogin ? 'Autenticando...' : 'Processando...';
                    }
                }
            }
        });
    });

    /* ==========================================================================
       EXTRA: ACESSIBILIDADE DE TECLADO PARA LINKS "SR-ONLY"
       Garante que o link de salto (Pular para conteúdo) funcione corretamente
       movendo o foco do teclado para o formulário.
       ========================================================================== */
    const skipLink = document.querySelector('.sr-only-focusable');
    if (skipLink) {
        skipLink.addEventListener('click', function(e) {
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                // Adiciona tabindex temporário para forçar o foco no elemento alvo
                targetElement.setAttribute('tabindex', '-1');
                targetElement.focus();
                targetElement.addEventListener('blur', function() {
                    targetElement.removeAttribute('tabindex');
                }, { once: true });
            }
        });
    }
});

/* ==========================================================================
       VALIDAÇÃO DE FORMULÁRIOS E PREVENÇÃO DE ERROS (Heurística 9 e WCAG)
       ========================================================================== */
    const requiredInputs = document.querySelectorAll('input[required]');

    // Função para mostrar ou esconder o erro
    const toggleError = (input, show) => {
        // Encontra a div .input-group pai do input
        const group = input.closest('.input-group');
        if (!group) return;

        // Encontra o span de erro específico deste input
        const errorSpan = group.querySelector('.text-danger');
        
        if (show) {
            if (errorSpan) errorSpan.style.display = 'block';
            input.classList.add('input-error');
            input.setAttribute('aria-invalid', 'true'); // WCAG: Avisa leitor de tela
        } else {
            if (errorSpan) errorSpan.style.display = 'none';
            input.classList.remove('input-error');
            input.removeAttribute('aria-invalid');
        }
    };

    requiredInputs.forEach(input => {
        // 1. Valida quando o usuário sai do campo (Blur)
        input.addEventListener('blur', function() {
            if (!this.validity.valid) {
                toggleError(this, true);
            }
        });

        // 2. Remove o erro instantaneamente quando o usuário começa a digitar/corrigir
        input.addEventListener('input', function() {
            if (this.validity.valid) {
                toggleError(this, false);
            }
        });

        // 3. Substitui o alerta padrão feio do navegador ao clicar em "Submit"
        input.addEventListener('invalid', function(e) {
            e.preventDefault(); // Impede o balão padrão do HTML5
            toggleError(this, true);
        });
    });