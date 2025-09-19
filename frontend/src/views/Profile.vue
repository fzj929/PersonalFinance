<template>
  <div class="profile-container">
    <van-nav-bar title="我的" />

    <div class="user-info">
      <van-cell center title="用户信息">
        <template #icon>
          <van-icon name="user-circle-o" size="24" style="margin-right: 8px;" />
        </template>
      </van-cell>

      <van-cell title="用户名" :value="user?.username || '未登录'" />
      <van-cell title="用户ID" :value="user?.id || '-'" />
    </div>

    <div class="actions">
      <van-cell-group>
        <van-cell title="分类管理" is-link @click="manageCategories">
          <template #icon>
            <van-icon name="label-o" size="18" style="margin-right: 8px;" />
          </template>
        </van-cell>
        <van-cell title="显示设置" is-link @click="showSettings">
          <template #icon>
            <van-icon name="setting-o" size="18" style="margin-right: 8px;" />
          </template>
        </van-cell>
        <van-cell title="数据导出" is-link @click="exportData">
          <template #icon>
            <van-icon name="down" size="18" style="margin-right: 8px;" />
          </template>
        </van-cell>
        <van-cell title="清除缓存" is-link @click="clearCache">
          <template #icon>
            <van-icon name="delete" size="18" style="margin-right: 8px;" />
          </template>
        </van-cell>
        <van-cell title="关于" is-link @click="showAbout">
          <template #icon>
            <van-icon name="info" size="18" style="margin-right: 8px;" />
          </template>
        </van-cell>
      </van-cell-group>
    </div>

    <div class="logout-section">
      <van-button
        round
        block
        type="danger"
        @click="handleLogout"
        :disabled="!user"
      >
        退出登录
      </van-button>
    </div>

    <van-tabbar v-model="active" route>
      <van-tabbar-item name="home" icon="home-o" to="/">首页</van-tabbar-item>
      <van-tabbar-item name="stats" icon="chart-trending-o" to="/stats">统计</van-tabbar-item>
      <van-tabbar-item name="profile" icon="user-o" to="/profile">我的</van-tabbar-item>
    </van-tabbar>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { showConfirmDialog, showNotify, showDialog } from 'vant'
import api from '../services/api'

const active = ref('profile')
const user = ref(null)
const router = useRouter()

const loadUserInfo = () => {
  const userData = localStorage.getItem('user')
  if (userData) {
    user.value = JSON.parse(userData)
  }
}

const handleLogout = async () => {
  try {
    await showConfirmDialog({
      message: '确定要退出登录吗？'
    })

    // 清除本地存储
    localStorage.removeItem('token')
    localStorage.removeItem('user')

    showNotify({ type: 'success', message: '已退出登录' })
    router.push('/login')
  } catch (error) {
    if (error !== 'cancel') {
      showNotify({ type: 'danger', message: '退出登录失败' })
    }
  }
}

const clearCache = async () => {
  try {
    await showConfirmDialog({
      message: '确定要清除缓存吗？这将清除所有本地数据。'
    })

    localStorage.clear()
    showNotify({ type: 'success', message: '缓存已清除' })
    router.push('/login')
  } catch (error) {
    if (error !== 'cancel') {
      showNotify({ type: 'danger', message: '清除缓存失败' })
    }
  }
}

const exportData = async () => {
  try {
    const response = await api.get('/transactions/export', {
      responseType: 'blob'
    })

    // 创建CSV文件并下载
    const blob = new Blob([response.data], { type: 'text/csv;charset=utf-8;' })
    const link = document.createElement('a')
    const url = URL.createObjectURL(blob)

    link.setAttribute('href', url)
    link.setAttribute('download', `transactions_${new Date().toISOString().split('T')[0]}.csv`)
    link.style.visibility = 'hidden'

    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)

    showNotify({ type: 'success', message: '数据导出成功' })
  } catch (error) {
    showNotify({ type: 'danger', message: error.response?.data || '数据导出失败' })
  }
}

const showAbout = () => {
  showDialog({
    title: '关于',
    message: '个人记账系统 v1.0\n\n开发团队: fengzhengjin929@163.com\n\n一款简洁易用的个人收支管理应用',
    messageAlign: 'left',
    confirmButtonText: '确定'
  })
}

const manageCategories = () => {
  showDialog({
    title: '分类管理',
    message: '分类管理功能将在后续版本中推出',
    confirmButtonText: '确定'
  })
}

const showSettings = () => {
  showDialog({
    title: '显示设置',
    message: '首页显示记录数设置功能将在后续版本中推出',
    confirmButtonText: '确定'
  })
}

onMounted(loadUserInfo)
</script>

<style scoped>
.profile-container {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 50px;
}

.user-info {
  margin-bottom: 16px;
}

.actions {
  margin-bottom: 16px;
}

.logout-section {
  padding: 0 16px;
}
</style>