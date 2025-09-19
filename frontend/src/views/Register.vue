<template>
  <div class="register-container">
    <van-nav-bar title="注册" left-text="返回" left-arrow @click-left="$router.back()" />

    <van-form @submit="onSubmit" class="register-form">
      <van-cell-group inset>
        <van-field
          v-model="form.username"
          name="username"
          label="用户名"
          placeholder="请输入用户名"
          :rules="[{ required: true, message: '请填写用户名' }]"
        />
        <van-field
          v-model="form.email"
          name="email"
          label="邮箱"
          placeholder="请输入邮箱"
          :rules="[{ required: true, message: '请填写邮箱' }]"
        />
        <van-field
          v-model="form.password"
          type="password"
          name="password"
          label="密码"
          placeholder="请输入密码"
          :rules="[{ required: true, message: '请填写密码' }]"
        />
        <van-field
          v-model="form.confirmPassword"
          type="password"
          name="confirmPassword"
          label="确认密码"
          placeholder="请再次输入密码"
          :rules="[{ required: true, message: '请确认密码' }]"
        />
      </van-cell-group>

      <div style="margin: 16px;">
        <van-button round block type="primary" native-type="submit" :loading="loading">
          注册
        </van-button>
      </div>
    </van-form>

    <div class="login-link">
      <span>已有账号？</span>
      <router-link to="/login">立即登录</router-link>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { showNotify } from 'vant'
import api from '../services/api'
import store from '../store/index.js'

const form = reactive({
  username: '',
  email: '',
  password: '',
  confirmPassword: ''
})
const loading = ref(false)
const router = useRouter()

const onSubmit = async () => {
  if (form.password !== form.confirmPassword) {
    showNotify({ type: 'danger', message: '两次密码输入不一致' })
    return
  }

  loading.value = true

  try {
    const response = await api.post('/auth/register', {
      username: form.username,
      email: form.email,
      password: form.password
    })

    // 保存token和用户信息
    localStorage.setItem('token', response.data.token)
    localStorage.setItem('user', JSON.stringify({
      id: response.data.userId,
      username: response.data.username
    }))

    store.commit('setToken', response.data.token)
    store.commit('setUser', {
      id: response.data.userId,
      username: response.data.username
    })

    showNotify({ type: 'success', message: '注册成功' })
    router.push('/')
  } catch (error) {
    showNotify({ type: 'danger', message: error.response?.data || '注册失败' })
  }

  loading.value = false
}
</script>

<style scoped>
.register-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.register-form {
  padding: 20px;
  margin-top: 50px;
}

.login-link {
  text-align: center;
  color: white;
  margin-top: 20px;
}

.login-link a {
  color: #ffd700;
  text-decoration: none;
}
</style>