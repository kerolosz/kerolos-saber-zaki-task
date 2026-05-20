import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { ProductList } from './components/product-list/product-list';
import { CommonModule } from '@angular/common';
import { CreateProduct } from './components/create-product/create-product';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [App, ProductList, CreateProduct],
  imports: [BrowserModule, AppRoutingModule, CommonModule, ReactiveFormsModule],
  providers: [provideBrowserGlobalErrorListeners()],
  bootstrap: [App],
})
export class AppModule {}
