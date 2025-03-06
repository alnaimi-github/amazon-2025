import { Component,inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Order } from '../../../shared/models/order';

@Component({
  selector: 'app-order-detialed',
  imports: [
    MatCardModule,
    MatButton,
    DatePipe,
    CurrencyPipe
  ],
  templateUrl: './order-detialed.component.html',
  styleUrl: './order-detialed.component.scss'
})
export class OrderDetialedComponent implements OnInit {
  private orderService = inject(OrderService);
  private activatedRoute = inject(ActivatedRoute);
  order?: Order;

  ngOnInit(): void {
    this.loadOrder();
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this.orderService.getOrderDetailed(+id).subscribe({
    next: order => this.order = order
    })
  }
}
