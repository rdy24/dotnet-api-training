# Cinema API Complete - APIdog Documentation

## Overview
Complete REST API for Cinema Management System with authentication, user management, studio management, schedules, tickets, and transaction processing.

**Base URL**: `https://localhost:7001`

## Authentication
All endpoints except login require Bearer token authentication.

### Header Format:
```
Authorization: Bearer {your_jwt_token}
```

## API Endpoints

### 🔐 Authentication
- `POST /api/auth/login` - User login
- `GET /api/auth/test-sentry` - Test Sentry integration

### 👥 Users Management (Admin/Customer)
- `GET /api/users` - Get all users (Admin only)
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create user (Admin only)
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user (Admin only)

### 🏢 Studios Management
- `GET /api/studios` - Get all studios
- `GET /api/studios/{id}` - Get studio by ID
- `POST /api/studios` - Create studio (Admin only)
- `PUT /api/studios/{id}` - Update studio (Admin only)
- `DELETE /api/studios/{id}` - Delete studio (Admin only)

### 🎬 Movies Management
- `GET /api/movies` - Get all movies
- `GET /api/movies/{id}` - Get movie by ID
- `POST /api/movies` - Create movie (Admin only)
- `PUT /api/movies/{id}` - Update movie (Admin only)
- `DELETE /api/movies/{id}` - Delete movie (Admin only)

### 📅 Schedules Management
- `GET /api/schedules` - Get all schedules
- `GET /api/schedules/{id}` - Get schedule by ID
- `POST /api/schedules` - Create schedule (Admin only)
- `PUT /api/schedules/{id}` - Update schedule (Admin only)
- `DELETE /api/schedules/{id}` - Delete schedule (Admin only)

### 🎫 Tickets Management
- `GET /api/tickets` - Get all tickets
- `GET /api/tickets/{id}` - Get ticket by ID
- `POST /api/tickets` - Create ticket
- `PUT /api/tickets/{id}` - Update ticket (Admin only)
- `DELETE /api/tickets/{id}` - Delete ticket (Admin only)

### 💰 Transactions Management
- `GET /api/transactions` - Get all transactions (Admin only)
- `GET /api/transactions/{id}` - Get transaction by ID
- `GET /api/transactions/user/{userId}` - Get transactions by user ID
- `GET /api/transactions/ticket/{ticketId}` - Get transactions by ticket ID
- `POST /api/transactions` - Create transaction
- `PUT /api/transactions/{id}` - Update transaction (Admin only)
- `DELETE /api/transactions/{id}` - Delete transaction (Admin only)

## Data Models

### PaymentMethod Enum
- `0` = Cash
- `1` = CreditCard
- `2` = DebitCard
- `3` = EWallet
- `4` = BankTransfer

### PaymentStatus Enum
- `0` = Pending
- `1` = Success
- `2` = Failed
- `3` = Cancelled

### TicketStatus Enum
- `0` = Reserved
- `1` = Confirmed
- `2` = Cancelled

### User Roles
- `Admin` - Full access to all endpoints
- `Customer` - Limited access to user-specific data

## Error Responses
All endpoints return standardized error responses:

```json
{
    "success": false,
    "message": "Error description",
    "details": "Detailed error information",
    "statusCode": 400,
    "timestamp": "2025-08-19T10:30:00.000Z",
    "path": "/api/endpoint",
    "validationErrors": []
}
```

## Success Responses
All endpoints return standardized success responses:

```json
{
    "success": true,
    "message": "Operation successful",
    "data": {...},
    "statusCode": 200,
    "timestamp": "2025-08-19T10:30:00.000Z",
    "path": "/api/endpoint"
}
```

## Background Services
- **Transaction Queue Service**: Automatically processes pending transactions
- **Transaction Scheduler Service**: Cleans up expired transactions every 6 hours

## Monitoring
- **Sentry Integration**: Error tracking and performance monitoring
- **Logging**: Comprehensive logging for all operations

## Environment Configuration
The API supports environment-based configuration:
- JWT tokens from environment variables
- Database connection strings
- Sentry DSN for error monitoring