<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Ошибка — Спектр</title>
    
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;600&family=Manrope:wght@700;800&family=PT+Sans:wght@400;700&display=swap" rel="stylesheet">
    
    <!-- Используем те же переменные и стили -->
    <link href="${url.resourcesPath}/css/styles.css" rel="stylesheet" />
    <style>
        .error-shell {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            background: var(--ink);
            background-image: linear-gradient(rgba(16,36,62,.05) 1px, transparent 1px), linear-gradient(90deg, rgba(16,36,62,.05) 1px, transparent 1px);
            background-size: 32px 32px;
            color: white;
            padding: 20px;
        }
        .error-card {
            background: rgba(19, 52, 82, 0.58);
            backdrop-filter: blur(12px);
            border: 1px solid rgba(255,255,255,0.11);
            border-radius: 12px;
            padding: 50px 40px;
            text-align: center;
            max-width: 480px;
            box-shadow: 0 18px 50px rgba(0, 0, 0, 0.4);
        }
        .error-card h1 {
            font-family: "Manrope", sans-serif;
            font-size: 28px;
            margin-bottom: 16px;
            letter-spacing: -0.03em;
            color: white;
        }
        .error-card p {
            font-size: 15px;
            color: #b8c7d5;
            margin-bottom: 35px;
            line-height: 1.6;
        }
        .back-button {
            display: inline-block;
            padding: 14px 28px;
            background: var(--teal-bright);
            color: var(--ink);
            text-decoration: none;
            border-radius: 6px;
            font-family: "Manrope", sans-serif;
            font-weight: 800;
            transition: all 0.2s ease;
        }
        .back-button:hover {
            background: var(--teal);
            color: white;
            transform: translateY(-2px);
        }
        .technical-error {
            margin-top: 30px;
            font-size: 12px;
            color: var(--red);
            font-family: "IBM Plex Mono", monospace;
            opacity: 0.7;
        }
    </style>
</head>
<body>
    <div class="error-shell">
        <div class="error-card">
            <div class="brand brand-light" style="justify-content: center; margin-bottom: 35px;">
                <span class="brand-mark"></span><span>Спектр</span>
            </div>
            
            <h1>Ссылка недоступна</h1>
            
            <p>
                Похоже, вы обновили страницу авторизации или перешли по устаревшей ссылке. 
                В целях безопасности сессия была сброшена. Пожалуйста, вернитесь на главную страницу и попробуйте войти снова.
            </p>
            
            <#if client?? && client.baseUrl?has_content>
                <a id="backToApplication" href="${client.baseUrl}" class="back-button">Вернуться в систему</a>
            <#else>
                <a id="backToApplication" href="http://localhost:5005" class="back-button">Вернуться в систему</a>
            </#if>

            <#if message?has_content>
                <div class="technical-error">
                    Код ошибки: ${kcSanitize(message.summary)?no_esc}
                </div>
            </#if>
        </div>
    </div>
</body>
</html>
