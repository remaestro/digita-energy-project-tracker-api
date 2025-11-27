using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DigitaEnergy.ProjectTracker.Application.Configuration;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace DigitaEnergy.ProjectTracker.Application.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendInvitationEmailAsync(string toEmail, string firstName, string lastName, string invitationToken, string invitedByName)
    {
        // Utilisation d'un fragment d'URL (#) au lieu d'un query param (?) pour éviter l'encodage par Gmail
        var acceptUrl = $"{_emailSettings.ApplicationUrl}/accept-invite#token={invitationToken}";
        var subject = "Invitation à rejoindre Digita Energy Project Tracker";
        
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2563eb; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9fafb; padding: 30px; border: 1px solid #e5e7eb; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #2563eb; color: white; text-decoration: none; border-radius: 6px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #6b7280; }}
        .info-box {{ background-color: #dbeafe; padding: 15px; border-left: 4px solid #2563eb; margin: 20px 0; }}
        .link-text {{ word-break: break-all; color: #2563eb; background-color: #eff6ff; padding: 10px; border-radius: 4px; font-family: monospace; font-size: 13px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Digita Energy Project Tracker</h1>
        </div>
        <div class='content'>
            <p>Bonjour {firstName} {lastName},</p>
            
            <p><strong>{invitedByName}</strong> vous a invité(e) à rejoindre le Project Tracker de Digita Energy.</p>
            
            <p>Pour créer votre compte et accéder à la plateforme, veuillez cliquer sur le bouton ci-dessous :</p>
            
            <div style='text-align: center;'>
                <!--[if mso]>
                <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{acceptUrl}"" style=""height:40px;v-text-anchor:middle;width:200px;"" arcsize=""10%"" strokecolor=""#2563eb"" fillcolor=""#2563eb"">
                  <w:anchorlock/>
                  <center style=""color:#ffffff;font-family:sans-serif;font-size:14px;font-weight:bold;"">Accepter l'invitation</center>
                </v:roundrect>
                <![endif]-->
                <!--[if !mso]><!-->
                <a href=""{acceptUrl}"" class='button' style=""color: #ffffff !important; text-decoration: none;"" target=""_blank"" rel=""noopener noreferrer"">Accepter l'invitation</a>
                <!--<![endif]-->
            </div>
            
            <div class='info-box'>
                <p><strong>Note importante :</strong> Ce lien d'invitation est valide pendant 7 jours. Après cette période, vous devrez demander une nouvelle invitation.</p>
            </div>
            
            <div style='background-color: #fef3c7; border: 2px solid #f59e0b; border-radius: 6px; padding: 15px; margin: 20px 0;'>
                <p style='margin: 0 0 10px 0; color: #92400e; font-weight: bold;'>⚠️ Important - Si le bouton ne fonctionne pas :</p>
                <p style='margin: 0 0 10px 0; color: #92400e;'>Copiez le lien ci-dessous et collez-le dans la barre d'adresse de votre navigateur :</p>
                <div style='background-color: #fffbeb; padding: 12px; border-radius: 4px; border: 1px solid #fbbf24;'>
                    <code style='word-break: break-all; color: #1e40af; font-size: 14px; font-family: monospace;'>{acceptUrl}</code>
                </div>
            </div>
            
            <p>Si vous n'avez pas demandé cette invitation, vous pouvez ignorer ce message en toute sécurité.</p>
            
            <p>Cordialement,<br/>L'équipe Digita Energy</p>
        </div>
        <div class='footer'>
            <p>© 2025 Digita Energy. Tous droits réservés.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        var resetUrl = $"{_emailSettings.ApplicationUrl}/reset-password#token={resetToken}";
        var subject = "Réinitialisation de votre mot de passe";
        
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #dc2626; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9fafb; padding: 30px; border: 1px solid #e5e7eb; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #dc2626; color: white; text-decoration: none; border-radius: 6px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #6b7280; }}
        .warning-box {{ background-color: #fef2f2; padding: 15px; border-left: 4px solid #dc2626; margin: 20px 0; }}
        .link-text {{ word-break: break-all; color: #dc2626; background-color: #fef2f2; padding: 10px; border-radius: 4px; font-family: monospace; font-size: 13px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Réinitialisation de mot de passe</h1>
        </div>
        <div class='content'>
            <p>Bonjour,</p>
            
            <p>Vous avez demandé la réinitialisation de votre mot de passe pour votre compte Digita Energy Project Tracker.</p>
            
            <p>Pour créer un nouveau mot de passe, veuillez cliquer sur le bouton ci-dessous :</p>
            
            <div style='text-align: center;'>
                <!--[if mso]>
                <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href=""{resetUrl}"" style=""height:40px;v-text-anchor:middle;width:250px;"" arcsize=""10%"" strokecolor=""#dc2626"" fillcolor=""#dc2626"">
                  <w:anchorlock/>
                  <center style=""color:#ffffff;font-family:sans-serif;font-size:14px;font-weight:bold;"">Réinitialiser mon mot de passe</center>
                </v:roundrect>
                <![endif]-->
                <!--[if !mso]><!-->
                <a href=""{resetUrl}"" class='button' style=""color: #ffffff !important; text-decoration: none;"" target=""_blank"" rel=""noopener noreferrer"">Réinitialiser mon mot de passe</a>
                <!--<![endif]-->
            </div>
            
            <div class='warning-box'>
                <p><strong>Sécurité :</strong> Ce lien est valide pendant 1 heure. Si vous n'avez pas demandé cette réinitialisation, veuillez ignorer ce message et votre mot de passe restera inchangé.</p>
            </div>
            
            <p>Si le bouton ne fonctionne pas, vous pouvez copier et coller ce lien dans votre navigateur :</p>
            <div class='link-text'>{resetUrl}</div>
            
            <p style='margin-top: 20px;'><strong>Astuce :</strong> Si le lien ne fonctionne pas directement depuis votre messagerie, copiez-le et collez-le dans la barre d'adresse de votre navigateur.</p>
            
            <p>Cordialement,<br/>L'équipe Digita Energy</p>
        </div>
        <div class='footer'>
            <p>© 2025 Digita Energy. Tous droits réservés.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string firstName)
    {
        var loginUrl = $"{_emailSettings.ApplicationUrl}/login";
        var subject = "Bienvenue sur Digita Energy Project Tracker";
        
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #059669; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: #f9fafb; padding: 30px; border: 1px solid #e5e7eb; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #059669; color: white; text-decoration: none; border-radius: 6px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #6b7280; }}
        .features {{ background-color: #d1fae5; padding: 15px; border-radius: 6px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Bienvenue !</h1>
        </div>
        <div class='content'>
            <p>Bonjour {firstName},</p>
            
            <p>Votre compte a été créé avec succès ! Bienvenue dans l'équipe Digita Energy Project Tracker.</p>
            
            <div class='features'>
                <h3>Que pouvez-vous faire maintenant ?</h3>
                <ul>
                    <li>Suivre l'avancement des tâches et jalons</li>
                    <li>Gérer les risques du projet</li>
                    <li>Collaborer avec votre équipe</li>
                    <li>Consulter les rapports et tableaux de bord</li>
                </ul>
            </div>
            
            <div style='text-align: center;'>
                <a href='{loginUrl}' class='button'>Se connecter maintenant</a>
            </div>
            
            <p>Si vous avez des questions ou besoin d'aide, n'hésitez pas à contacter votre chef de projet.</p>
            
            <p>Cordialement,<br/>L'équipe Digita Energy</p>
        </div>
        <div class='footer'>
            <p>© 2025 Digita Energy. Tous droits réservés.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmail));

            using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.EnableSsl,
                Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
            };

            await smtpClient.SendMailAsync(message);
            Console.WriteLine($"[EMAIL] Email sent successfully to {toEmail}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EMAIL] Failed to send email to {toEmail}: {ex.Message}");
            // Ne pas lever d'exception pour ne pas bloquer le workflow principal
            // En production, on pourrait logger dans un système de monitoring
        }
    }
}
