
export interface Order {
  id: number;
  orderDate: string;
  buyerEmail: string;
  deliveryMethod: string;
  shippingAddress: ShippingAddress;
  paymentSummary: PaymentSummary;
  orderItems: OrderItem[];
  shippingPrice: number;
  subtotal: number;
  total: number;
  status: string;
  paymentIntentId: string;

}

export interface ShippingAddress {
  name: string
  line1: string
  line2?: string
  city: string
  state: string
  postalCode: string
  country: string
}

export interface PaymentSummary {
  last4: number
  brand: string
  expMonth: number
  expYear: number
}

export interface OrderItem {
  productId: number
  productName: string
  pictureUrl: string
  price: number
  quantity: number
}


export interface OrderToCreate {
 cartId: string;
 deliveryMethodId: number;
 shippingAddress: ShippingAddress;
 paymentSummary: PaymentSummary;
}
