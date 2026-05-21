# SSO Implementation with Azure AD (ASP.NET Core API + Angular)

A complete step-by-step guide to implement Single Sign-On using Microsoft Azure Active Directory.

---

## Table of Contents

1. [Create an Azure Account](#1-create-an-azure-account)
2. [Register Your Applications on Azure Portal](#2-register-your-applications-on-azure-portal)
3. [Configure the API App Registration](#3-configure-the-api-app-registration)
4. [Configure the Angular App Registration](#4-configure-the-angular-app-registration)
5. [Setup ASP.NET Core API](#5-setup-aspnet-core-api)
6. [Setup Angular Application](#6-setup-angular-application)
7. [Test the SSO Flow](#7-test-the-sso-flow)

---

## 1. Create an Azure Account

### Step 1.1 — Sign up for Azure

1. Go to [https://azure.microsoft.com/free](https://azure.microsoft.com/free)
2. Click **"Start free"**
3. Sign in with an existing Microsoft account, or create a new one
4. Fill in your details and credit card info (you won't be charged — it's for identity verification only)
5. You get **$200 free credit** for 30 days + free services for 12 months

> ✅ If your company already has an Azure subscription, ask your IT admin to add you as a user — skip this step.

---

## 2. Register Your Applications on Azure Portal

You need **two App Registrations**:

- One for the **ASP.NET Core API** (the backend)
- One for the **Angular App** (the frontend)

### Step 2.1 — Open Azure Portal

1. Go to [https://portal.azure.com](https://portal.azure.com)
2. Sign in with your Microsoft account

### Step 2.2 — Navigate to App Registrations

1. In the search bar at the top, type **"App registrations"**
2. Click on it from the results
3. You will see a list (empty if new account)

---

## 3. Configure the API App Registration

### Step 3.1 — Create API Registration

1. Click **"+ New registration"**
2. Fill in the form:

```
Name:                    MyApp.API
Supported account types: Accounts in this organizational directory only (Single tenant)
Redirect URI:            (leave empty for now)
```

3. Click **"Register"**

### Step 3.2 — Copy Important IDs

After registration, you will see the **Overview** page. Copy these values:

```
Application (client) ID  →  xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Directory (tenant) ID    →  xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
```

> ⚠️ Save these — you will need them in the code later.

### Step 3.3 — Expose an API (Create a Scope)

1. In the left menu, click **"Expose an API"**
2. Click **"+ Add a scope"**
3. It will ask you to set an Application ID URI — click **"Save and continue"** (keep the default)
4. Fill in the scope form:

```
Scope name:             access_as_user
Who can consent:        Admins and users
Admin consent display name:   Access the API
Admin consent description:    Allows the app to access the API
User consent display name:    Access the API
User consent description:     Allows you to access the API
State:                  Enabled
```

5. Click **"Add scope"**

> ✅ Note the full scope URI — it looks like:
> `api://xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/access_as_user`

---

## 4. Configure the Angular App Registration

### Step 4.1 — Create Angular Registration

1. Go back to **"App registrations"**
2. Click **"+ New registration"** again
3. Fill in the form:

```
Name:                    MyApp.Angular
Supported account types: Accounts in this organizational directory only (Single tenant)
Redirect URI:            Single-page application (SPA) → http://localhost:4200
```

4. Click **"Register"**

### Step 4.2 — Copy the Client ID

From the **Overview** page, copy:

```
Application (client) ID  →  yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy
```

### Step 4.3 — Add API Permission

1. In the left menu, click **"API permissions"**
2. Click **"+ Add a permission"**
3. Click **"My APIs"** tab
4. Select **"MyApp.API"**
5. Check the **"access_as_user"** scope
6. Click **"Add permissions"**
7. Click **"Grant admin consent for [your tenant]"** → Click **"Yes"**

> ✅ The status should now show a green checkmark.

### Step 4.4 — Add Logout URL

1. In the left menu, click **"Authentication"**
2. Under **"Single-page application"**, find **Redirect URIs**
3. Add: `http://localhost:4200`
4. Under **"Front-channel logout URL"**, add: `http://localhost:4200/logout`
5. Click **"Save"**

---

## 5. Setup ASP.NET Core API

### Step 5.1 — Install NuGet Package

Run this command in your API project directory:

```bash
dotnet add package Microsoft.Identity.Web
```

### Step 5.2 — Update appsettings.json

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_API_CLIENT_ID",
    "Audience": "api://YOUR_API_CLIENT_ID"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> Replace `YOUR_TENANT_ID` and `YOUR_API_CLIENT_ID` with the values you copied in Step 3.2

### Step 5.3 — Update Program.cs

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Azure AD Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

// ✅ Add CORS to allow Angular to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAngular");     // Must be before UseAuthentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Step 5.4 — Protect Your Controllers

Add `[Authorize]` attribute to any controller or action you want to protect:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]                          // ← Requires valid Azure AD token
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        // Get the logged-in user's info from the token
        var userId = User.FindFirst("oid")?.Value;
        var userName = User.Identity?.Name;

        return Ok(new { message = "Protected data", user = userName });
    }

    [AllowAnonymous]                 // ← This action is public (no login needed)
    [HttpGet("public")]
    public IActionResult GetPublic()
    {
        return Ok("This is public");
    }
}
```

---

## 6. Setup Angular Application

### Step 6.1 — Install MSAL Packages

Run these commands in your Angular project directory:

```bash
npm install @azure/msal-browser @azure/msal-angular
```

### Step 6.2 — Create Auth Config File

Create a new file `src/app/auth-config.ts`:

```typescript
import { Configuration, BrowserCacheLocation } from "@azure/msal-browser";

export const msalConfig: Configuration = {
  auth: {
    clientId: "YOUR_ANGULAR_CLIENT_ID", // From Step 4.2
    authority: "https://login.microsoftonline.com/YOUR_TENANT_ID", // From Step 3.2
    redirectUri: "http://localhost:4200",
    postLogoutRedirectUri: "http://localhost:4200",
  },
  cache: {
    cacheLocation: BrowserCacheLocation.LocalStorage,
  },
};

// The API scope you created in Step 3.3
export const apiScopes = ["api://YOUR_API_CLIENT_ID/access_as_user"];

// The API base URL
export const apiBaseUrl = "https://localhost:7000/api";
```

### Step 6.3 — Update app.config.ts

```typescript
import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { provideRouter } from "@angular/router";
import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { MsalModule, MsalInterceptor } from "@azure/msal-angular";
import { PublicClientApplication, InteractionType } from "@azure/msal-browser";
import { routes } from "./app.routes";
import { msalConfig, apiScopes, apiBaseUrl } from "./auth-config";

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    // ✅ Setup MSAL
    importProvidersFrom(
      MsalModule.forRoot(
        new PublicClientApplication(msalConfig),
        {
          interactionType: InteractionType.Redirect, // Redirect to Microsoft login page
          authRequest: { scopes: apiScopes },
        },
        {
          interactionType: InteractionType.Redirect,
          protectedResourceMap: new Map([
            // Auto-attach token to all requests to your API
            [apiBaseUrl, apiScopes],
          ]),
        },
      ),
    ),

    // ✅ HTTP interceptor that auto-attaches the Bearer token
    provideHttpClient(withInterceptors([MsalInterceptor as any])),
  ],
};
```

### Step 6.4 — Update app.component.ts

```typescript
import { Component, OnInit } from "@angular/core";
import { MsalService, MsalBroadcastService } from "@azure/msal-angular";
import { InteractionStatus } from "@azure/msal-browser";
import { filter } from "rxjs/operators";
import { CommonModule } from "@angular/common";

@Component({
  selector: "app-root",
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="isLoggedIn">
      <p>Welcome, {{ userName }}!</p>
      <button (click)="logout()">Logout</button>
    </div>
    <div *ngIf="!isLoggedIn">
      <button (click)="login()">Login with Microsoft</button>
    </div>
  `,
})
export class AppComponent implements OnInit {
  isLoggedIn = false;
  userName = "";

  constructor(
    private msalService: MsalService,
    private broadcastService: MsalBroadcastService,
  ) {}

  ngOnInit(): void {
    // Wait until MSAL is ready
    this.broadcastService.inProgress$
      .pipe(filter((status) => status === InteractionStatus.None))
      .subscribe(() => {
        const accounts = this.msalService.instance.getAllAccounts();
        this.isLoggedIn = accounts.length > 0;
        if (this.isLoggedIn) {
          this.userName = accounts[0].name ?? accounts[0].username;
        }
      });
  }

  login(): void {
    this.msalService.loginRedirect();
  }

  logout(): void {
    this.msalService.logoutRedirect();
  }
}
```

### Step 6.5 — Call the Protected API

In any Angular service or component:

```typescript
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { apiBaseUrl } from "../auth-config";

@Injectable({ providedIn: "root" })
export class ProductsService {
  constructor(private http: HttpClient) {}

  // The MSAL interceptor will automatically add the Bearer token
  getProducts(): Observable<any> {
    return this.http.get(`${apiBaseUrl}/products`);
  }
}
```

---

## 7. Test the SSO Flow

### Step 7.1 — Run Both Applications

**Terminal 1 — Run the API:**

```bash
dotnet run
# API running at https://localhost:7000
```

**Terminal 2 — Run Angular:**

```bash
ng serve
# App running at http://localhost:4200
```

### Step 7.2 — Test the Login Flow

1. Open `http://localhost:4200` in the browser
2. Click **"Login with Microsoft"**
3. You will be redirected to Microsoft's login page
4. Enter your Microsoft/work account credentials
5. After successful login, you are redirected back to the app ✅
6. The Angular app now has a token and will send it with every API request

### Step 7.3 — Verify Token in API

Add a temporary debug endpoint to verify everything works:

```csharp
[Authorize]
[HttpGet("me")]
public IActionResult GetCurrentUser()
{
    var claims = User.Claims.Select(c => new { c.Type, c.Value });
    return Ok(claims);
}
```

Call `GET https://localhost:7000/api/products/me` — you should see your Azure AD claims.

---

## Summary — What You Built

```
User clicks "Login with Microsoft"
            ↓
   Angular redirects to Microsoft
            ↓
   User enters their credentials
            ↓
   Microsoft validates and returns a Token
            ↓
   Angular stores the Token (LocalStorage)
            ↓
   Every HTTP request to the API automatically includes:
   Authorization: Bearer <token>
            ↓
   API validates the token with Azure AD ✅
            ↓
   User gets the protected data
```

> This is a complete SSO flow using **OpenID Connect** (OIDC) + **OAuth 2.0**, which is the industry standard used by ADFS, Azure AD, Google, and all major identity providers.
