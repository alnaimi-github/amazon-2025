
export interface CartType
{
  id: string;
  cartItems: CartItem[];
}

export interface CartItem{
  productName: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand: string;
  type: string;
  productType: string;
  productId: number;
  productBrandId: number;
}

export class Cart implements CartType
{
  id = '';
  items: CartItem[] = [];
}
