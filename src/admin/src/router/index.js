import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../components/Home.vue'
import Login from '../components/Login.vue'
import Menu from '../components/system/Menu.vue'
import User from '../components/system/User.vue'
import Cate from '../components/category/Cate.vue'
import List from '../components/article/List.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    redirect: '/login'
  },
  {
    path: '/login',
    component: Login
  },
  {
    path: '/home',
    component: Home,
    children: [
      { path: '/menus', component: Menu },
      { path: '/users', component: User },
      { path: '/categories', component: Cate },
      { path: '/articles', component: List }
    ]
  }
]

const router = new VueRouter({
  routes
})

export default router
