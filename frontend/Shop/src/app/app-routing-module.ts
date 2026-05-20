import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductList } from './components/product-list/product-list';
import { CreateProduct } from './components/create-product/create-product';

const routes: Routes = [
  { path: '', component: ProductList, pathMatch: 'full' },
  { path: 'addProduct', component: CreateProduct },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
