
```json
{
  "id": "evt_3ab4e0d5d27445cf8a52ab3d8cb8f0b1",
  "type": "collection.succeeded",
  "created_at": "2026-09-22T10:41:18.402Z",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "charge_id": "ch_1a2b3c4d5e6f",
    "checkout_id": "chk_6R7s8T9u0V1w2X3y",
    "reference": "ORD-20260922-1041",
    "status": "SUCCEEDED",
    "amount": "75000.00",
    "currency": "NGN",
    "settlement_amount": "73875.00",
    "settlement_currency": "NGN",
    "processing_fee": "1125.00",
    "processing_fee_currency": "NGN",
    "fee_bearer": "merchant",
    "product_cart": [
      { "product_id": "prod_1a2b3c", "quantity": 2 },
      { "product_id": "prod_9x8y7z", "quantity": 1, "amount": "5000.00" }
    ],
    "customer": {
      "id": "cust_xyz789",
      "email": "jane@example.com",
      "name": "Jane Doe"
    },
    "payment_method_details": {
      "type": "bank_transfer",
      "bank_transfer": {
        "sender_name": "JANE ADEYEMI",
        "sender_bank": "Guaranty Trust Bank",
        "sender_bank_code": "058",
        "sender_account_number": "2294879124",
        "session_id": "000013260922104115000821734502",
        "narration": "ORD-20260922-1041",
        "virtual_account": {
          "id": null,
          "account_number": "9902847361",
          "bank_name": "Example Bank",
          "type": "one_time",
          "expires_at": "2026-09-22T11:11:18.402Z"
        }
      }
    },
    "metadata": {
      "order_id": "ORD-20260922-1041"
    }
  }
}
```

```json
{
  "id": "evt_87f6f546ba7148ffb1ecaf2a23f11a2f",
  "type": "collection.failed",
  "created_at": "2026-09-22T10:43:11.102Z",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "charge_id": "ch_1a2b3c4d5e6f",
    "checkout_id": "chk_6R7s8T9u0V1w2X3y",
    "reference": "ORD-20260922-1041",
    "status": "FAILED",
    "amount": "75000.00",
    "currency": "NGN",
    "settlement_amount": "73875.00",
    "settlement_currency": "NGN",
    "processing_fee": null,
    "processing_fee_currency": null,
    "fee_bearer": "merchant",
    "product_cart": [
      { "product_id": "prod_1a2b3c", "quantity": 2 }
    ],
    "customer": {
      "id": "cust_xyz789",
      "email": "jane@example.com",
      "name": "Jane Doe"
    },
    "reason": "Payment authorization failed",
    "metadata": {}
  }
}
```

```json
{
  "id": "evt_6d5c4b3a2f1e0d9c8b7a6f5e4d3c2b1a",
  "type": "collection.underpaid",
  "created_at": "2026-02-22T17:10:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "charge_id": "ch_1a2b3c4d5e6f",
    "reference": "order_1042",
    "checkout_id": "chk_6R7s8T9u0V1w2X3y",
    "amount_paid": "50000.00",
    "amount_expected": "75000.00",
    "amount_remaining": "25000.00",
    "currency": "NGN",
    "status": "UNDERPAID",
    "metadata": {}
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "customer.subscription.created",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "subscription_id": "sub_1a2b3c4d5e",
    "customer": {
      "customer_id": "cust_1a2b3c4d5e6f",
      "email": "jane@example.com",
      "name": "Jane Doe",
      "phone_number": "+2348012345678",
      "metadata": {},
      "billing_address": {
        "line1": "40 Yaba Road",
        "line2": null,
        "city": "Lagos",
        "state": "Lagos",
        "postal_code": "101245",
        "country": "NG"
      },
      "created_at": "2026-03-01T12:00:00Z",
      "updated_at": "2026-03-01T12:00:00Z"
    },
    "product_id": "prod_abc123",
    "status": "active",
    "collection_method": "charge_automatically",
    "currency": "USD",
    "amount": "10.00",
    "billing_cycle": { "interval": "month", "frequency": 1 },
    "quantity": 1,
    "current_period_start": "2026-04-01T00:00:00Z",
    "current_period_end": "2026-05-01T00:00:00Z",
    "next_billed_at": "2026-05-01T00:00:00Z",
    "trial_end": null,
    "cancel_at_period_end": false,
    "canceled_at": null,
    "created_at": "2026-03-01T12:00:00Z",
    "items": [],
    "metadata": {}
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "customer.subscription.updated",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "subscription_id": "sub_1a2b3c4d5e",
    "customer": {
      "customer_id": "cust_1a2b3c4d5e6f",
      "email": "jane@example.com",
      "name": "Jane Doe",
      "phone_number": "+2348012345678",
      "metadata": {},
      "billing_address": {
        "line1": "40 Yaba Road",
        "line2": null,
        "city": "Lagos",
        "state": "Lagos",
        "postal_code": "101245",
        "country": "NG"
      },
      "created_at": "2026-03-01T12:00:00Z",
      "updated_at": "2026-03-01T12:00:00Z"
    },
    "product_id": "prod_abc123",
    "status": "active",
    "collection_method": "charge_automatically",
    "currency": "USD",
    "amount": "10.00",
    "billing_cycle": { "interval": "month", "frequency": 1 },
    "quantity": 1,
    "current_period_start": "2026-04-01T00:00:00Z",
    "current_period_end": "2026-05-01T00:00:00Z",
    "next_billed_at": "2026-05-01T00:00:00Z",
    "trial_end": null,
    "cancel_at_period_end": false,
    "canceled_at": null,
    "created_at": "2026-03-01T12:00:00Z",
    "items": [],
    "metadata": {}
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "customer.subscription.deleted",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "subscription_id": "sub_1a2b3c4d5e",
    "customer": {
      "customer_id": "cust_1a2b3c4d5e6f",
      "email": "jane@example.com",
      "name": "Jane Doe",
      "phone_number": "+2348012345678",
      "metadata": {},
      "billing_address": {
        "line1": "40 Yaba Road",
        "line2": null,
        "city": "Lagos",
        "state": "Lagos",
        "postal_code": "101245",
        "country": "NG"
      },
      "created_at": "2026-03-01T12:00:00Z",
      "updated_at": "2026-03-01T12:00:00Z"
    },
    "product_id": "prod_abc123",
    "status": "canceled",
    "collection_method": "charge_automatically",
    "currency": "USD",
    "amount": "10.00",
    "billing_cycle": { "interval": "month", "frequency": 1 },
    "quantity": 1,
    "current_period_start": "2026-04-01T00:00:00Z",
    "current_period_end": "2026-05-01T00:00:00Z",
    "next_billed_at": "2026-05-01T00:00:00Z",
    "trial_end": null,
    "cancel_at_period_end": false,
    "canceled_at": "2026-04-15T00:00:00Z",
    "created_at": "2026-03-01T12:00:00Z",
    "items": [],
    "metadata": {}
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "invoice.created",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "invoice_id": "inv_1a2b3c4d5e",
    "subscription": { "subscription_id": "sub_1a2b3c4d5e" },
    "customer": { "customer_id": "cust_1a2b3c4d5e6f", "email": "jane@example.com", "name": "Jane Doe" },
    "charge": null,
    "status": "open",
    "collection_method": "charge_automatically",
    "currency": "USD",
    "subtotal": "10.00",
    "total": "10.00",
    "amount_paid": "0.00",
    "amount_remaining": "10.00",
    "period_start": "2026-04-01T00:00:00Z",
    "period_end": "2026-05-01T00:00:00Z",
    "attempt_count": 0,
    "next_payment_attempt": "2026-04-01T00:05:00Z",
    "created_at": "2026-04-01T00:00:00Z",
    "metadata": {}
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "invoice.paid",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "invoice_id": "inv_1a2b3c4d5e",
    "subscription": { "subscription_id": "sub_1a2b3c4d5e" },
    "customer": { "customer_id": "cust_1a2b3c4d5e6f", "email": "jane@example.com", "name": "Jane Doe" },
    "charge": null,
    "status": "paid",
    "collection_method": "charge_automatically",
    "currency": "USD",
    "subtotal": "10.00",
    "total": "10.00",
    "amount_paid": "10.00",
    "amount_remaining": "0.00",
    "period_start": "2026-04-01T00:00:00Z",
    "period_end": "2026-05-01T00:00:00Z",
    "attempt_count": 0,
    "next_payment_attempt": "2026-04-01T00:05:00Z",
    "created_at": "2026-04-01T00:00:00Z",
    "metadata": {}
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "invoice.payment_failed",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "invoice_id": "inv_1a2b3c4d5e",
    "subscription": { "subscription_id": "sub_1a2b3c4d5e" },
    "customer": { "customer_id": "cust_1a2b3c4d5e6f", "email": "jane@example.com", "name": "Jane Doe" },
    "charge": null,
    "status": "open",
    "collection_method": "charge_automatically",
    "currency": "USD",
    "subtotal": "10.00",
    "total": "10.00",
    "amount_paid": "0.00",
    "amount_remaining": "10.00",
    "period_start": "2026-04-01T00:00:00Z",
    "period_end": "2026-05-01T00:00:00Z",
    "attempt_count": 1,
    "next_payment_attempt": "2026-04-01T00:05:00Z",
    "created_at": "2026-04-01T00:00:00Z",
    "metadata": {}
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "payout.created",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "withdrawal_id": "pay_4Xr9dLc0mNv7Kq2B",
    "reference": "payout_9876",
    "status": "pending",
    "amount": "500.00",
    "currency": "USD",
    "from_currency": "USD",
    "to_currency": "NGN",
    "exchange_rate": "1650.00",
    "to_amount": "825000.00",
    "withdrawal_fee": "2.50",
    "net_from_amount": "497.50"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "payout.paid",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "withdrawal_id": "pay_4Xr9dLc0mNv7Kq2B",
    "reference": "payout_9876",
    "status": "completed",
    "amount": "500.00",
    "currency": "USD",
    "from_currency": "USD",
    "to_currency": "NGN",
    "exchange_rate": "1650.00",
    "to_amount": "825000.00",
    "withdrawal_fee": "2.50",
    "net_from_amount": "497.50"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "payout.failed",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "withdrawal_id": "pay_4Xr9dLc0mNv7Kq2B",
    "reference": "payout_9876",
    "status": "failed",
    "amount": "500.00",
    "currency": "USD",
    "from_currency": "USD",
    "to_currency": "NGN",
    "exchange_rate": "1650.00",
    "to_amount": "825000.00",
    "withdrawal_fee": "2.50",
    "net_from_amount": "497.50"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "refund.created",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "refund_id": "ref_1a2b3c4d5e",
    "charge_id": "ch_1a2b3c4d5e6f",
    "reference": "refund_9876",
    "status": "processing",
    "requested_amount": "10.00",
    "refunded_amount": "0.00",
    "refund_fee_amount": "0.00",
    "fee_bearer": "merchant",
    "reason": "Customer request"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "refund.paid",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "refund_id": "ref_1a2b3c4d5e",
    "charge_id": "ch_1a2b3c4d5e6f",
    "reference": "refund_9876",
    "status": "paid",
    "requested_amount": "10.00",
    "refunded_amount": "10.00",
    "refund_fee_amount": "0.00",
    "fee_bearer": "merchant",
    "reason": "Customer request"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "refund.failed",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "refund_id": "ref_1a2b3c4d5e",
    "charge_id": "ch_1a2b3c4d5e6f",
    "reference": "refund_9876",
    "status": "failed",
    "requested_amount": "10.00",
    "refunded_amount": "0.00",
    "refund_fee_amount": "0.00",
    "fee_bearer": "merchant",
    "reason": "Customer request"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "dispute.created",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "account": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "dispute_id": "dsp_1a2b3c4d5e",
    "charge_id": "ch_9f4c1d2e7b6a4f8e9c0d1a2b3c4d5e6f",
    "amount": "75000.00",
    "currency": "NGN",
    "status": "needs_response",
    "is_response_editable": true,
    "reason": "fraudulent",
    "response_deadline_at": "2026-05-10T00:00:00Z",
    "created_at": "2026-04-27T12:00:00Z",
    "updated_at": "2026-04-27T12:00:00Z",
    "charge_amount": "75000.00",
    "charge_currency": "NGN"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "dispute.updated",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "account": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "dispute_id": "dsp_1a2b3c4d5e",
    "charge_id": "ch_9f4c1d2e7b6a4f8e9c0d1a2b3c4d5e6f",
    "amount": "75000.00",
    "currency": "NGN",
    "status": "under_review",
    "is_response_editable": true,
    "reason": "fraudulent",
    "response_deadline_at": "2026-05-10T00:00:00Z",
    "created_at": "2026-04-27T12:00:00Z",
    "updated_at": "2026-04-27T12:00:00Z",
    "charge_amount": "75000.00",
    "charge_currency": "NGN"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "conversion.completed",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "conversion_id": "cnv_1a2b3c4d5e",
    "quote_id": "cq_1a2b3c4d5e",
    "from_currency": "USD",
    "to_currency": "NGN",
    "from_amount": "100.00",
    "to_amount": "165000.00",
    "exchange_rate": "1650.00",
    "status": "completed",
    "created_at": "2026-04-27T12:00:00Z"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "conversion.failed",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "conversion_id": "cnv_1a2b3c4d5e",
    "quote_id": "cq_1a2b3c4d5e",
    "from_currency": "USD",
    "to_currency": "NGN",
    "from_amount": "100.00",
    "to_amount": "165000.00",
    "exchange_rate": "1650.00",
    "status": "failed",
    "created_at": "2026-04-27T12:00:00Z"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "customer.created",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "customer_id": "cust_1a2b3c4d5e6f",
    "email": "jane@example.com",
    "name": "Jane Doe",
    "phone_number": "+2348012345678",
    "metadata": {},
    "billing_address": {
      "line1": "40 Yaba Road",
      "line2": null,
      "city": "Lagos",
      "state": "Lagos",
      "postal_code": "101245",
      "country": "NG"
    },
    "created_at": "2026-04-27T12:00:00Z",
    "updated_at": "2026-04-27T12:00:00Z"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "customer.updated",
  "created_at": "2026-04-27T12:00:00.000000+00:00",
  "organization_id": "acct_7KpQ2mNv4XbR9dLc",
  "data": {
    "customer_id": "cust_1a2b3c4d5e6f",
    "email": "jane@example.com",
    "name": "Jane Doe",
    "phone_number": "+2348012345678",
    "metadata": {},
    "billing_address": {
      "line1": "40 Yaba Road",
      "line2": null,
      "city": "Lagos",
      "state": "Lagos",
      "postal_code": "101245",
      "country": "NG"
    },
    "created_at": "2026-04-27T12:00:00Z",
    "updated_at": "2026-04-27T12:00:00Z"
  }
}
```

```json
{
  "id": "evt_1a2b3c4d5e6f7g8h",
  "type": "account.updated",
  "created_at": "2026-08-07T11:04:22.518000+00:00",
  "organization_id": "acct_3Wq8ZfT1yHnJ5sVe",
  "account": "acct_3Wq8ZfT1yHnJ5sVe",
  "data": {
    "account": "acct_3Wq8ZfT1yHnJ5sVe",
    "outstanding": ["company.registration_number"]
  }
}
```

```json
{
  "id": "evt_2b3c4d5e6f7g8h9i",
  "type": "capability.updated",
  "created_at": "2026-08-07T11:06:41.204000+00:00",
  "organization_id": "acct_3Wq8ZfT1yHnJ5sVe",
  "account": "acct_3Wq8ZfT1yHnJ5sVe",
  "data": {
    "account": "acct_3Wq8ZfT1yHnJ5sVe",
    "capability": "payouts",
    "status": "active",
    "requested": true
  }
}
```

```json
{
  "id": "evt_3c4d5e6f7g8h9i0j",
  "type": "transfer.created",
  "created_at": "2026-08-07T11:04:22.518000+00:00",
  "organization_id": "acct_3Wq8ZfT1yHnJ5sVe",
  "account": "acct_3Wq8ZfT1yHnJ5sVe",
  "data": {
    "transfer_id": "tr_8c1e04a7b93f2d6540ab",
    "source": "acct_7KpQ2mNv4XbR9dLc",
    "destination": "acct_3Wq8ZfT1yHnJ5sVe",
    "amount": "7000.00",
    "currency": "NGN",
    "description": "Order #4471 seller share",
    "metadata": {},
    "kind": "payout",
    "source_charge_id": "ch_9f21c4e05b8a",
    "created_at": "2026-08-07T11:04:22.518000+00:00"
  }
}
```

```json

```

```json

```

```json

```