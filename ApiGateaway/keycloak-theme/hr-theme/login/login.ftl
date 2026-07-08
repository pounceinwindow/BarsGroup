<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Вход в систему — Спектр</title>
    
    <!-- Google Fonts required for the design -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;600&family=Manrope:wght@700;800&family=PT+Sans:wght@400;700&display=swap" rel="stylesheet">
    
    <!-- Custom Theme CSS -->
    <link href="${url.resourcesPath}/css/styles.css" rel="stylesheet" />
</head>
<body>
<main class="login-shell">
    <section class="login-manifesto" aria-label="О системе">
        <div class="brand brand-light"><span class="brand-mark"></span><span>Спектр</span></div>

        <div class="manifesto-copy">
            <span class="eyebrow">Система оценки кандидатов</span>
            <h1>Каждое собеседование — на своей шкале</h1>
            <p>Единый контур для кандидатов, матриц компетенций и решений по найму.</p>
        </div>

        <div class="spectrum-card" aria-label="Шкала компетенций">
            <#list [
                {"name": "C# Basics", "score": 4},
                {"name": "OOP", "score": 3},
                {"name": "SQL Basics", "score": 5},
                {"name": "Git", "score": 4},
                {"name": "Algorithms", "score": 3},
                {"name": "Advanced C#", "score": 4},
                {"name": "Entity Framework", "score": 3},
                {"name": "REST API", "score": 5},
                {"name": "Unit Testing", "score": 4},
                {"name": "Design Patterns", "score": 3}
            ] as comp>
                <div class="spectrum-row">
                    <span class="spectrum-index">${(comp?index + 1)?string("00")}</span>
                    <span>${comp.name}</span>
                    <div class="dot-scale">
                        <#list 1..5 as i>
                            <span class="dot ${ (i <= comp.score)?string('active', '') }"></span>
                        </#list>
                    </div>
                </div>
            </#list>
        </div>
    </section>

    <section class="login-panel">
        <div class="login-card">
            <div class="brand brand-mobile"><span class="brand-mark"></span><span>Спектр</span></div>
            <span class="eyebrow dark">Демо-доступ</span>
            <h2>Вход в систему</h2>
            <p class="form-intro">Введите данные вашей учетной записи для авторизации.</p>

            <#if message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
                <#if message.type == 'error'>
                    <div style="color: var(--red); background: var(--red-pale); padding: 12px; border-radius: 5px; margin-bottom: 20px; font-size: 14px; border: 1px solid rgba(184, 61, 51, 0.2);">
                        ${kcSanitize(message.summary)?no_esc}
                    </div>
                </#if>
            </#if>

            <form action="${url.loginAction}" method="post">
                <label for="username">Логин</label>
                <input id="username" name="username" type="text" autocomplete="username" autofocus value="${(login.username!'')}" />

                <label for="password">Пароль</label>
                <input id="password" name="password" type="password" autocomplete="current-password" />

                <button type="submit" class="primary-button login-button">
                    Войти <span>в систему</span>
                </button>
            </form>
            <p class="demo-note">Защищено протоколом OIDC (Keycloak).</p>
        </div>
    </section>
</main>
</body>
</html>
