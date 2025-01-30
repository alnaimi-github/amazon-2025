import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Product } from '../../shared/models/product';
import { Pagination } from '../../shared/models/pagination';
import { ShopPramas } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://localhost:5000/api/';
  private readonly http = inject(HttpClient);
  types: string [] = [];
  brands: string [] = [];

  getproducts(shopPramas: ShopPramas){
    let params = new HttpParams();

    if(shopPramas.brands && shopPramas.brands.length> 0){
      params = params.append('brands',shopPramas.brands.join(','));
    }
    if(shopPramas.types && shopPramas.types.length> 0){
      params = params.append('types',shopPramas.types.join(','));
    }
   if(shopPramas.sort){
      params = params.append('sort',shopPramas.sort);
   }
   if(shopPramas.search){
    params = params.append('search',shopPramas.search);
   }
    params = params.append('pageSize',shopPramas.pageSize);
    params = params.append('pageIndex',shopPramas.pageNumber);

   return this.http.get<Pagination<Product>>(this.baseUrl + 'products',{params});
  }

  getProduct(id: number){
    return this.http.get<Product>(this.baseUrl + 'products/' + id);
  }

  getBrands(){
    if(this.brands.length > 0) {
      return;
    }
    return this.http.get<string[]>(this.baseUrl + 'products/brands').subscribe({
      next: response => this.brands = response
    })
    }

      getTypes(){
        if(this.types.length > 0) return;
        return this.http.get<string[]>(this.baseUrl + 'products/types').subscribe({
          next: response => this.types = response
        })
      }
  }

