# Configuration de l'envoi d'emails

Le service d'email a été implémenté avec succès ! 🎉

## Configuration nécessaire

Pour activer l'envoi d'emails, vous devez configurer les paramètres SMTP dans `appsettings.Development.json` :

```json
"EmailSettings": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUsername": "your-email@gmail.com",
  "SmtpPassword": "your-app-password",
  "FromEmail": "noreply@digita-energy.com",
  "FromName": "Digita Energy Project Tracker",
  "EnableSsl": true,
  "ApplicationUrl": "http://localhost:4200"
}
```

## Options de configuration SMTP

### Option 1 : Gmail (Recommandé pour le développement)

1. Activer l'authentification à deux facteurs sur votre compte Gmail
2. Générer un "App Password" : https://myaccount.google.com/apppasswords
3. Utiliser ce mot de passe dans `SmtpPassword`

```json
"SmtpHost": "smtp.gmail.com",
"SmtpPort": 587,
"SmtpUsername": "votre-email@gmail.com",
"SmtpPassword": "xxxx xxxx xxxx xxxx"
```

### Option 2 : Microsoft Outlook / Office 365

```json
"SmtpHost": "smtp.office365.com",
"SmtpPort": 587,
"SmtpUsername": "votre-email@outlook.com",
"SmtpPassword": "votre-mot-de-passe"
```

### Option 3 : SendGrid (Recommandé pour la production)

```json
"SmtpHost": "smtp.sendgrid.net",
"SmtpPort": 587,
"SmtpUsername": "apikey",
"SmtpPassword": "votre-api-key-sendgrid"
```

### Option 4 : Mailgun

```json
"SmtpHost": "smtp.mailgun.org",
"SmtpPort": 587,
"SmtpUsername": "postmaster@votre-domaine.mailgun.org",
"SmtpPassword": "votre-mot-de-passe-mailgun"
```

### Option 5 : MailDev (Pour tester localement sans envoyer de vrais emails)

1. Installer MailDev : `npm install -g maildev`
2. Lancer : `maildev`
3. Configuration :

```json
"SmtpHost": "localhost",
"SmtpPort": 1025,
"SmtpUsername": "",
"SmtpPassword": "",
"EnableSsl": false
```

Interface web disponible sur : http://localhost:1080

## Fonctionnalités implémentées

Le service envoie automatiquement des emails pour :

1. **Invitation d'utilisateur** : Email avec lien d'acceptation (valide 7 jours)
2. **Réinitialisation de mot de passe** : Email avec lien de réinitialisation (valide 1 heure)
3. **Bienvenue** : Email de confirmation après création du compte

## Gestion des erreurs

Le système est conçu pour ne pas bloquer le workflow si l'envoi d'email échoue. Les erreurs sont loggées mais n'empêchent pas la création d'invitation ou la réinitialisation de mot de passe.

## Test sans configuration SMTP

Si vous ne configurez pas de SMTP valide, l'application fonctionnera normalement mais les emails ne seront pas envoyés. Les messages d'erreur seront visibles dans les logs :

```
[EMAIL] Failed to send email to user@example.com: Unable to connect to SMTP server
```

## Production

Pour la production, il est recommandé d'utiliser :
- **SendGrid** : Jusqu'à 100 emails/jour gratuits
- **Mailgun** : Jusqu'à 5000 emails/mois gratuits
- **AWS SES** : Très économique pour gros volumes
- **Azure Communication Services** : Intégration Azure native

## Variables d'environnement (optionnel)

Vous pouvez aussi utiliser des variables d'environnement au lieu du fichier de configuration :

```bash
export EmailSettings__SmtpHost="smtp.gmail.com"
export EmailSettings__SmtpPort="587"
export EmailSettings__SmtpUsername="your-email@gmail.com"
export EmailSettings__SmtpPassword="your-app-password"
```
