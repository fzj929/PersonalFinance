<template>
  <div class="query-container">
    <van-nav-bar
      title="明细查询"
      left-text="返回"
      left-arrow
      @click-left="$router.back()"
    />

    <div class="filter-section">
      <van-cell-group inset>
        <van-field
          v-model="filters.startDate"
          is-link
          readonly
          label="开始日期"
          placeholder="选择开始日期"
          @click="showStartDatePicker = true"
        />
        <van-field
          v-model="filters.endDate"
          is-link
          readonly
          label="结束日期"
          placeholder="选择结束日期"
          @click="showEndDatePicker = true"
        />
        <van-field
          v-model="filters.category"
          is-link
          readonly
          label="分类"
          placeholder="选择分类"
          @click="showCategoryPicker = true"
        />
        <van-field
          v-model="filters.type"
          label="类型"
        >
          <template #input>
            <van-radio-group v-model="filters.type" direction="horizontal">
              <van-radio name="">全部</van-radio>
              <van-radio name="income">收入</van-radio>
              <van-radio name="expense">支出</van-radio>
            </van-radio-group>
          </template>
        </van-field>
      </van-cell-group>

      <div style="margin: 16px;">
        <van-button round block type="primary" @click="searchTransactions" :loading="loading">
          查询
        </van-button>
      </div>
    </div>

    <div class="results-section">
      <van-list
        v-model:loading="loading"
        :finished="finished"
        finished-text="没有更多了"
        @load="loadTransactions"
      >
        <van-cell-group inset>
          <van-cell v-for="transaction in transactions" :key="transaction.id" :title="formatTransaction(transaction)">
            <template #value>
              <span :class="{ 'income-text': transaction.type === 'income', 'expense-text': transaction.type === 'expense' }">
                {{ transaction.type === 'income' ? '+' : '-' }}{{ transaction.amount }}
              </span>
            </template>
            <template #label>
              <div>{{ transaction.category }} · {{ formatDate(transaction.transactionDate) }}</div>
              <div v-if="transaction.description" class="description">{{ transaction.description }}</div>
            </template>
          </van-cell>
        </van-cell-group>
      </van-list>
    </div>

    <!-- Date Pickers -->
    <van-popup v-model:show="showStartDatePicker" round position="bottom">
      <van-date-picker
        v-model="startDateValue"
        @confirm="onConfirmStartDate"
        @cancel="showStartDatePicker = false"
      />
    </van-popup>

    <van-popup v-model:show="showEndDatePicker" round position="bottom">
      <van-date-picker
        v-model="endDateValue"
        @confirm="onConfirmEndDate"
        @cancel="showEndDatePicker = false"
      />
    </van-popup>

    <van-popup v-model:show="showCategoryPicker" round position="bottom">
      <van-picker
        :columns="categoryOptions"
        @confirm="onConfirmCategory"
        @cancel="showCategoryPicker = false"
      />
    </van-popup>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { showNotify } from 'vant'
import dayjs from 'dayjs'
import api from '../services/api'

const loading = ref(false)
const finished = ref(false)
const showStartDatePicker = ref(false)
const showEndDatePicker = ref(false)
const showCategoryPicker = ref(false)

const startDateValue = ref([dayjs().subtract(30, 'day').format('YYYY'), dayjs().subtract(30, 'day').format('MM'), dayjs().subtract(30, 'day').format('DD')])
const endDateValue = ref([dayjs().format('YYYY'), dayjs().format('MM'), dayjs().format('DD')])

const filters = reactive({
  startDate: dayjs().subtract(30, 'day').format('YYYY-MM-DD'),
  endDate: dayjs().format('YYYY-MM-DD'),
  category: '',
  type: ''
})

const transactions = ref([])
const categoryOptions = ref([
  { text: '全部', value: '' },
  { text: '餐饮', value: '餐饮' },
  { text: '交通', value: '交通' },
  { text: '购物', value: '购物' },
  { text: '娱乐', value: '娱乐' },
  { text: '医疗', value: '医疗' },
  { text: '教育', value: '教育' },
  { text: '住房', value: '住房' },
  { text: '工资', value: '工资' },
  { text: '奖金', value: '奖金' },
  { text: '投资', value: '投资' },
  { text: '其他', value: '其他' }
])

const onConfirmStartDate = (value) => {
  filters.startDate = `${value.selectedValues[0]}-${value.selectedValues[1]}-${value.selectedValues[2]}`
  showStartDatePicker.value = false
}

const onConfirmEndDate = (value) => {
  filters.endDate = `${value.selectedValues[0]}-${value.selectedValues[1]}-${value.selectedValues[2]}`
  showEndDatePicker.value = false
}

const onConfirmCategory = (value) => {
  filters.category = value.selectedOptions[0].value
  showCategoryPicker.value = false
}

const formatDate = (dateString) => {
  return dayjs(dateString).format('MM-DD')
}

const formatTransaction = (transaction) => {
  return `${transaction.type === 'income' ? '收入' : '支出'} · ${transaction.category}`
}

const searchTransactions = async () => {
  transactions.value = []
  finished.value = false
  await loadTransactions()
}

const loadTransactions = async () => {
  if (loading.value) return

  loading.value = true

  try {
    const params = new URLSearchParams()
    if (filters.startDate) params.append('startDate', filters.startDate)
    if (filters.endDate) params.append('endDate', filters.endDate)
    if (filters.category) params.append('category', filters.category)
    if (filters.type) params.append('type', filters.type)

    const response = await api.get(`/transactions?${params}`)
    transactions.value = response.data
    finished.value = true
  } catch (error) {
    showNotify({ type: 'danger', message: '查询失败' })
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadTransactions()
})
</script>

<style scoped>
.query-container {
  min-height: 100vh;
  background-color: #f5f5f5;
}

.filter-section {
  margin-bottom: 16px;
}

.income-text {
  color: #07c160;
  font-weight: bold;
}

.expense-text {
  color: #ee0a24;
  font-weight: bold;
}

.description {
  color: #969799;
  font-size: 12px;
  margin-top: 4px;
}
</style>