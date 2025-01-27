import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ProductItemComponent } from "./product-item/product-item.component";
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';

@Component({
  selector: 'app-shop',
  imports: [
    CommonModule,
    ProductItemComponent,
    MatIcon,
    MatButton,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger
],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {

  private readonly shopService = inject(ShopService);
  private readonly dialogService = inject(MatDialog);
  products: Product[] = [];
  selectedBrands: string[] = [];
  selectedTypes: string[] = [];
  selectedSort: string ='name';
  sortOptions = [
    {name:'Alphabetical', value:'name'},
    {name:'Price: Low-Heigh', value:'priceAsc'},
    {name:'Price: High-Low', value:'priceDesc'}
  ]

  ngOnInit(): void {
   this.initializeShop();
  }

  initializeShop(){
    this.shopService.getBrands();
    this.shopService.getTypes();
    this.getProducts();

  }

  openFiltersDialog(){
    const dialogRef = this.dialogService.open(FiltersDialogComponent,{
      minWidth:'500px',
      data: {
        selectedBrands: this.selectedBrands,
        selectedTypes: this.selectedTypes
      }
    });
    dialogRef.afterClosed().subscribe({
      next: result => {
        if(result){
          console.log(result);
          this.selectedBrands = result.selectedBrands;
          this.selectedTypes = result.selectedTypes;
          this.getProducts();
        }
      }
    })
  }

  getProducts(){
    this.shopService.getproducts(this.selectedBrands,this.selectedTypes,this.selectedSort).subscribe( {
      next: response => this.products = response.data,
      error: error => console.error(error)
    });
  }

  onSortChange(event: MatSelectionListChange){
     const selectedOption = event.options[0];
     if(selectedOption){
       this.selectedSort = selectedOption.value;
       this.getProducts();
     }
  }

}
