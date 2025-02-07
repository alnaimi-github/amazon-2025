
export interface User{
  firstName: string;
  lastName: string;
  email: string;
  address: Address;
}

export interface Address{
  line1: string;
  line2?: string;
  city: string;
  postalCode: string;
  country: string;
  state: string;
}
