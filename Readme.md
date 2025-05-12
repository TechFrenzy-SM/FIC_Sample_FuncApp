# Accessing Azure Storage Across Tenants Using Federated Identity Credentials and User-Managed Identities

## Solution
This scenario demonstrates how to access an Azure Storage Account in one tenant (Tenant_2) from an Azure Function App in another tenant (Tenant_1) using Federated Identity Credentials (FIC) and user-managed identities.

---

## Resources

### Tenant_1:
- **User-Managed Identity**
- **Azure Function App**
- **App Registration (Multi-Tenant App)**

### Tenant_2:
- **Storage Account**

---

## Steps

### In Tenant_1:
1. **Create a User-Managed Identity**  
   - Create a user-managed identity in Tenant_1.

2. **Deploy an Azure Function App**  
   - Deploy an Azure Function App in Tenant_1 and assign the user-managed identity to it.

3. **Create a Multi-Tenant App Registration**  
   - Register a multi-tenant app in Tenant_1.

4. **Add the User-Managed Identity as a Federated Identity Credential (FIC)**  
   - Under the "Secrets and Certificates" section of the app registration, add the user-managed identity as a Federated Identity Credential.

---

### In Tenant_2:
1. **Access the Multi-Tenant App**  
   - Access the multi-tenant app registration from Tenant_2 and log in using Tenant_2 credentials.

2. **Register the Service Principal**  
   - Once logged in, the service principal of the multi-tenant app will automatically get registered in Tenant_2.

3. **Assign IAM Role to the Service Principal**  
   - Navigate to the Storage Account in Tenant_2.
   - Add the service principal of the multi-tenant app to the IAM section of the Storage Account.
   - Assign the **"Storage Blob Data Contributor"** role to the service principal.

---

## Outcome
By following these steps, you can access the Storage Account in Tenant_2 from the Function App in Tenant_1 using Federated Identity Credentials and user-managed identities.

This setup ensures secure and seamless cross-tenant access to Azure resources.

