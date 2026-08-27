# SimplePos REST API List

Full list of REST APIs derived from the domain model. Endpoints marked with ✅ already exist; those marked with 🔲 still need to be implemented.

---

## 1. Authentication & Registration

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| ✅ | `POST` | `/api/auth/login` | Login (returns JWT token) |
| 🔲 | `GET` | `/api/auth/me` | Get current logged-in user profile, company/outlet info, and permissions |
| ✅ | `POST` | `/api/register-business/` | Register a new business (company + admin user) |

---

## 2. Companies (`/api/companies`)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| ✅ | `POST` | `/api/companies/` | Create a company |
| ✅ | `GET` | `/api/companies/{id}` | Get company by ID |
| 🔲 | `PUT` | `/api/companies/{id}` | Update company info (name, address, phone) |
| 🔲 | `PUT` | `/api/companies/{id}/email` | Update company email |
| 🔲 | `PATCH` | `/api/companies/{id}/activate` | Activate company |
| 🔲 | `PATCH` | `/api/companies/{id}/deactivate` | Deactivate company |
| 🔲 | `DELETE` | `/api/companies/{id}` | Soft-delete company |

---

## 3. Outlets (`/api/outlets`)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/outlets/` | Create outlet |
| 🔲 | `GET` | `/api/outlets/{id}` | Get outlet by ID |
| 🔲 | `GET` | `/api/outlets?companyId={companyId}` | Get outlets by company |
| 🔲 | `PUT` | `/api/outlets/{id}` | Update outlet info (name, address, phone) |
| 🔲 | `PATCH` | `/api/outlets/{id}/activate` | Activate outlet |
| 🔲 | `PATCH` | `/api/outlets/{id}/deactivate` | Deactivate outlet |
| 🔲 | `PATCH` | `/api/outlets/{id}/last-online` | Update last online timestamp |
| 🔲 | `DELETE` | `/api/outlets/{id}` | Soft-delete outlet |

---

## 4. Categories (`/api/categories`)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/categories/` | Create category |
| 🔲 | `GET` | `/api/categories/{id}` | Get category by ID |
| 🔲 | `GET` | `/api/categories?companyId={companyId}` | Get categories by company |
| 🔲 | `PUT` | `/api/categories/{id}` | Update category name |
| 🔲 | `PATCH` | `/api/categories/{id}/activate` | Activate category |
| 🔲 | `PATCH` | `/api/categories/{id}/deactivate` | Deactivate category |
| 🔲 | `DELETE` | `/api/categories/{id}` | Soft-delete category |

---

## 5. Products (`/api/products`)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/products/` | Create product |
| 🔲 | `GET` | `/api/products/{id}` | Get product by ID |
| 🔲 | `GET` | `/api/products?companyId={companyId}` | Get products by company |
| 🔲 | `GET` | `/api/products?categoryId={categoryId}` | Get products by category |
| 🔲 | `GET` | `/api/products/sku/{sku}?companyId={companyId}` | Get product by SKU |
| 🔲 | `PUT` | `/api/products/{id}` | Update product info (SKU, name, cost, base price) |
| 🔲 | `PATCH` | `/api/products/{id}/activate` | Activate product |
| 🔲 | `PATCH` | `/api/products/{id}/deactivate` | Deactivate product |
| 🔲 | `DELETE` | `/api/products/{id}` | Soft-delete product |

### Product Taxes (sub-resource)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/products/{id}/taxes` | Add tax to product |
| 🔲 | `DELETE` | `/api/products/{id}/taxes/{taxId}` | Remove tax from product |

---

## 6. Taxes (`/api/taxes`)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/taxes/` | Create tax |
| 🔲 | `GET` | `/api/taxes/{id}` | Get tax by ID |
| 🔲 | `GET` | `/api/taxes?companyId={companyId}` | Get taxes by company |
| 🔲 | `GET` | `/api/taxes/active?companyId={companyId}` | Get active taxes by company |
| 🔲 | `PUT` | `/api/taxes/{id}` | Update tax info (name, rate) |
| 🔲 | `PATCH` | `/api/taxes/{id}/activate` | Activate tax |
| 🔲 | `PATCH` | `/api/taxes/{id}/deactivate` | Deactivate tax |
| 🔲 | `DELETE` | `/api/taxes/{id}` | Soft-delete tax |

---

## 7. Payment Methods (`/api/payment-methods`)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/payment-methods/` | Create payment method |
| 🔲 | `GET` | `/api/payment-methods/{id}` | Get payment method by ID |
| 🔲 | `GET` | `/api/payment-methods?companyId={companyId}` | Get payment methods by company |
| 🔲 | `GET` | `/api/payment-methods/active?companyId={companyId}` | Get active payment methods by company |
| 🔲 | `PUT` | `/api/payment-methods/{id}` | Update payment method name |
| 🔲 | `PATCH` | `/api/payment-methods/{id}/activate` | Activate payment method |
| 🔲 | `PATCH` | `/api/payment-methods/{id}/deactivate` | Deactivate payment method |
| 🔲 | `DELETE` | `/api/payment-methods/{id}` | Soft-delete payment method |

---

## 8. Users (`/api/users`)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/users/` | Create user |
| 🔲 | `GET` | `/api/users/{id}` | Get user by ID |
| 🔲 | `GET` | `/api/users/username/{username}` | Get user by username |
| 🔲 | `GET` | `/api/users/email/{email}` | Get user by email |
| 🔲 | `GET` | `/api/users?outletId={outletId}` | Get users by outlet |
| 🔲 | `PUT` | `/api/users/{id}` | Update user info (email, phone, position, active status) |
| 🔲 | `DELETE` | `/api/users/{id}` | Soft-delete user |

### User Permissions (sub-resource)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/users/{id}/permissions` | Assign permission to user |
| 🔲 | `DELETE` | `/api/users/{id}/permissions/{permissionId}` | Revoke permission from user |

---

## 9. Permissions (`/api/permissions`)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `GET` | `/api/permissions/` | Get all permissions |
| 🔲 | `GET` | `/api/permissions/{id}` | Get permission by ID |

> [!NOTE]
> Permission CRUD (create/update/delete) is commented out in [IPermissionRepository.cs](file:///Users/cheong/Projects/SimplePos/src/SimplePos.Domain/Permissions/IPermissionRepository.cs), suggesting permissions are seeded/managed internally rather than via API.

---

## 10. Sales (`/api/sales`)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/sales/` | Create a new sale |
| 🔲 | `GET` | `/api/sales/{id}` | Get sale by ID (with items & payments) |
| 🔲 | `GET` | `/api/sales?outletId={outletId}` | Get sales by outlet |
| 🔲 | `GET` | `/api/sales?outletId={outletId}&from={date}&to={date}` | Get sales by date range |
| 🔲 | `GET` | `/api/sales?userId={userId}` | Get sales by user |
| 🔲 | `PATCH` | `/api/sales/{id}/void` | Void a sale |
| 🔲 | `PATCH` | `/api/sales/{id}/unvoid` | Unvoid a sale |
| 🔲 | `PATCH` | `/api/sales/{id}/close` | Close a sale |
| 🔲 | `DELETE` | `/api/sales/{id}` | Soft-delete sale |

### Sale Items (sub-resource)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/sales/{id}/items` | Add item to sale |
| 🔲 | `PUT` | `/api/sales/{saleId}/items/{itemId}` | Update sale item (qty, price, discount, remark, taxes) |
| 🔲 | `PATCH` | `/api/sales/{saleId}/items/{itemId}/quantity` | Update sale item quantity |
| 🔲 | `PATCH` | `/api/sales/{saleId}/items/{itemId}/void` | Void a sale item |
| 🔲 | `PATCH` | `/api/sales/{saleId}/items/{itemId}/unvoid` | Unvoid a sale item |
| 🔲 | `DELETE` | `/api/sales/{saleId}/items/{itemId}` | Remove item from sale |

### Sale Payments (sub-resource)

| Status | Method | Endpoint | Description |
|--------|--------|----------|-------------|
| 🔲 | `POST` | `/api/sales/{id}/payments` | Add payment to sale |
| 🔲 | `GET` | `/api/sales/{id}/payments` | Get payments for a sale |
| 🔲 | `DELETE` | `/api/sales/{saleId}/payments/{paymentId}` | Remove payment from sale |

---

## Summary

| Domain | Implemented | Pending | Total |
|--------|:-----------:|:-------:|:-----:|
| Auth & Registration | 2 | 1 | 3 |
| Companies | 2 | 5 | 7 |
| Outlets | 0 | 8 | 8 |
| Categories | 0 | 7 | 7 |
| Products | 0 | 11 | 11 |
| Taxes | 0 | 8 | 8 |
| Payment Methods | 0 | 8 | 8 |
| Users | 0 | 9 | 9 |
| Permissions | 0 | 2 | 2 |
| Sales | 0 | 18 | 18 |
| **Total** | **4** | **77** | **81** |
