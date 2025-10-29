Volunteer Management System:
Overview:
A Volunteer Management System for managing volunteers and requests in an organization. Volunteers can select requests, update their status, and administrators can manage both volunteers and requests.

Features:
Volunteer can pick one request at a time and report completion or cancellation

Administrator can manage requests and volunteers

System maintains a separate system clock for time simulation

Request history is tracked for auditing

Volunteers can filter requests by distance

Roles:

Administrator: 
manages volunteers and requests, can also act as a volunteer

Volunteer: 
selects requests to handle, updates status, sets personal preferences

Build and run the application according to your environment (e.g., Java, .NET, Node.js)

Usage:
Administrator logs in and creates requests with maximum completion times

Volunteer logs in, filters requests by distance, and selects a request

Volunteer completes or cancels the request

Administrator monitors request statuses

Architecture:
Layered architecture (presentation, business logic, data)

Time simulation via a separate system clock

History of assignments maintained for auditing

Example Workflow:
Admin creates a food delivery request
Volunteer sees the request and accepts it
Volunteer completes the request
Admin checks request completion
