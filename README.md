# SecurePassManager - Mini Project

## Introduction

SecurePassManager is a secure password manager built with .NET, designed to help users safely store and manage their credentials using a local database.

## Security Model

### Objective

The primary goal is to safeguard user credentials against unauthorized access and data breaches. 

### Cryptography in Use

1. **Cryptographic Random Number Generator (CRNG)**:
   - The .NET `RNGCryptoServiceProvider` class will be used to generate secure random numbers.

2. **Shared-key (Symmetric) Cipher**:
   - The choice of encryption is AES-256, specifically in GCM mode for its added authentication capabilities. .NET's `AesGcm` class can be used for this purpose. Each password is encrypted before storage, using a unique IV generated for each session to enhance security.

3. **Hashing**:
   - While not directly storing passwords, SHA512 hashing will be applied, provided by .NET's `SHA512Managed` class, for creating secure digests when needed, such as for verifying the integrity of data.

4. **Message Authentication Code (MAC)**:
   - HMAC using SHA256 for verifying the authenticity and integrity of stored data. .NET's `HMACSHA256` class is utilized for this purpose, ensuring that the data has not been tampered with.

### .NET and Local Database Integration

The sokution will be developed with .NET framework, utilizing local databases for storing encrypted passwords securely. 

### Security Considerations

The design mitigates risks associated with:
- **Brute Force Attacks**: By implementing secure password policies and encryption.
- **Data Breaches**: Through AES-256 encryption and secure storage practices.
- **User Error**: Via a user-friendly interface that guides secure password practices.

## Diagrams

