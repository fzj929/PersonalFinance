<template>
  <div class="login-container">
    <van-nav-bar title="登录" />

    <van-form @submit="onSubmit" class="login-form">
      <van-cell-group inset>
        <van-field
          v-model="username"
          name="username"
          label="用户名"
          placeholder="请输入用户名"
          :rules="[{ required: true, message: '请填写用户名' }]"
        />
        <van-field
          v-model="password"
          type="password"
          name="password"
          label="密码"
          placeholder="请输入密码"
          :rules="[{ required: true, message: '请填写密码' }]"
        />
      </van-cell-group>

      <div style="margin: 16px;">
        <van-button round block type="primary" native-type="submit" :loading="loading">
          登录
        </van-button>
      </div>
    </van-form>

    <div class="register-link">
      <span>没有账号？</span>
      <router-link to="/register">立即注册</router-link>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { showNotify } from 'vant'
import api from '../services/api'
import store from '../store/index.js'

const username = ref('')
const password = ref('')
const loading = ref(false)
const router = useRouter()

const onSubmit = async () => {
  loading.value = true

  try {
    const response = await api.post('/auth/login', { username: username.value, password: password.value })

    localStorage.setItem('token', response.data.token)
    localStorage.setItem('user', JSON.stringify({ username: username.value }))

    store.commit('setToken', response.data.token)
    store.commit('setUser', { username: username.value })

    showNotify({ type: 'success', message: '登录成功' })
    router.push('/')
  } catch (error) {
    showNotify({ type: 'danger', message: error.response?.data || '登录失败' })
  }

  loading.value = false
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-form {
  padding: 20px;
  margin-top: 50px;
}

.register-link {
  text-align: center;
  color: white;
  margin-top: 20px;
}

.register-link a {
  color: #ffd700;
  text-decoration: none;
}
</style>