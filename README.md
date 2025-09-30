**Entities and Relations**
1. User: Id, Username, Email, PasswordHash, Role, IsEmailVerified, EmailVerificationToken, IsRequestingAuthorization, CreatedAt, LastLogin, IsDeleted
2. RefreshToken: Id, Token, UserId, CreatedAt, ExpiresAt, RevokedAt, IsActive
3. AuditLog: Id, UserId, Action, Endpoint, Metadata, TimeStamp
4. Events: -
5. Tickets: -
6. Order: -

**Endpoints:**
**AuthController**
POST api/auth/register
POST api/auth/verify-email
POST api/auth/login	email
POST api/auth/refresh
POST api/auth/logout

**UserController**
User	GET api/users/me
User	PATCH api/users/me/change-username
User	PATCH api/users/me/change-password
User	PATCH api/users/me/change-authorization-request
Admin	GET api/users/admin - Applied pagination, searching, sorting, and filtering
Admin	GET api/users/admin/{id}
Admin	PATCH api/users/admin/{id}/set-role
Admin	PATCH api/users/admin/{id}/set-authorization-request
Admin	PATCH api/users/admin/{id}/set-deleted
Admin	DELETE api/users/admin/{id}	id
Admin	GET api/users/admin/{id}/insights/activity
Admin	GET api/users/admin/{id}/insights/stats - for later
