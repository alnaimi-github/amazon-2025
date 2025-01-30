import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Product } from '../../shared/models/product';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ProductItemComponent } from "./product-item/product-item.component";
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import { ShopPramas } from '../../shared/models/shopParams';
import { Pagination } from '../../shared/models/pagination';
import { FormsModule } from '@angular/forms';

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
    MatMenuTrigger,
    MatPaginator,
    FormsModule
],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {

  private readonly shopService = inject(ShopService);
  private readonly dialogService = inject(MatDialog);
  products?: Pagination<Product> ;
  pageSizeOptions = [5,10,15,20];

  sortOptions = [
    {name:'Alphabetical', value:'name'},
    {name:'Price: Low-Heigh', value:'priceAsc'},
    {name:'Price: High-Low', value:'priceDesc'}
  ];

  shopPramas = new ShopPramas();

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
        selectedBrands: this.shopPramas.brands,
        selectedTypes: this.shopPramas.types
      }
    });
    dialogRef.afterClosed().subscribe({
      next: result => {
        if(result){
          console.log(result);
          this.shopPramas.brands = result.selectedBrands;
          this.shopPramas.types = result.selectedTypes;
          this.shopPramas.pageNumber = 1;
          this.getProducts();
        }
      }
    })
  }

  getProducts(){
    this.shopService.getproducts(this.shopPramas).subscribe( {
      next: response => this.products = response,
      error: error => console.error(error)
    });
  }

  onSortChange(event: MatSelectionListChange){
     const selectedOption = event.options[0];
     if(selectedOption){
       this.shopPramas.sort = selectedOption.value;
       this.shopPramas.pageNumber = 1;
       this.getProducts();
     }
  }

  handlePageEvent(event: PageEvent){
    this.shopPramas.pageNumber = event.pageIndex + 1;
    this.shopPramas.pageSize = event.pageSize;
    this.getProducts();
  }

  onSearchChange(){
    this.shopPramas.pageNumber = 1;
    this.getProducts();
  }

}
