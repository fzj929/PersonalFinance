<template>
  <div class="add-container">
    <van-nav-bar
      title="添加记录"
      left-text="返回"
      left-arrow
      @click-left="$router.back()"
    />

    <van-form @submit="onSubmit" class="add-form">
      <van-cell-group inset>
        <van-field
          v-model="form.amount"
          name="amount"
          label="金额"
          placeholder="请输入金额"
          type="number"
          :rules="[{ required: true, message: '请输入金额' }]"
        />

        <van-field
          v-model="form.type"
          name="type"
          label="类型"
          :rules="[{ required: true, message: '请选择类型' }]"
        >
          <template #input>
            <van-radio-group v-model="form.type" direction="horizontal">
              <van-radio name="income">收入</van-radio>
              <van-radio name="expense">支出</van-radio>
            </van-radio-group>
          </template>
        </van-field>

        <van-field
          v-model="form.category"
          is-link
          readonly
          name="category"
          label="分类"
          placeholder="请选择分类"
          @click="showCategoryPicker = true"
          :rules="[{ required: true, message: '请选择分类' }]"
        />

        <van-field
          v-model="form.description"
          name="description"
          label="备注"
          placeholder="请输入备注信息"
          type="textarea"
          rows="2"
          autosize
        />

        <van-field
          v-model="form.transactionDate"
          is-link
          readonly
          name="transactionDate"
          label="日期"
          placeholder="请选择日期"
          @click="showDatePicker = true"
          :rules="[{ required: true, message: '请选择日期' }]"
        />
      </van-cell-group>

      <div style="margin: 16px;">
        <van-button round block type="primary" native-type="submit" :loading="loading">
          保存
        </van-button>
      </div>
    </van-form>

    <van-popup v-model:show="showCategoryPicker" round position="bottom">
      <van-picker
        :columns="categoryOptions"
        @confirm="onConfirmCategory"
        @cancel="showCategoryPicker = false"
      />
    </van-popup>

    <van-popup v-model:show="showDatePicker" round position="bottom">
      <van-date-picker
        v-model="currentDate"
        @confirm="onConfirmDate"
        @cancel="showDatePicker = false"
      />
    </van-popup>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { showNotify } from 'vant'
import dayjs from 'dayjs'
import api from '../services/api'

const router = useRouter()
const loading = ref(false)
const showCategoryPicker = ref(false)
const showDatePicker = ref(false)
const currentDate = ref([
  dayjs().format('YYYY'),
  dayjs().format('MM'),
  dayjs().format('DD')
])

const form = reactive({
  amount: '',
  type: 'expense',
  category: '',
  description: '',
  transactionDate: dayjs().format('YYYY-MM-DD')
})

const categoryOptions = [
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
]

const onConfirmCategory = (value) => {
  form.category = value.selectedOptions[0].text
  showCategoryPicker.value = false
}

const onConfirmDate = (value) => {
  form.transactionDate = `${value.selectedValues[0]}-${value.selectedValues[1]}-${value.selectedValues[2]}`
  showDatePicker.value = false
}

const onSubmit = async () => {
  loading.value = true

  try {
    const transactionData = {
      ...form,
      amount: parseFloat(form.amount),
      transactionDate: new Date(form.transactionDate)
    }

    await api.post('/transactions', transactionData)
    showNotify({ type: 'success', message: '添加成功' })
    router.back()
  } catch (error) {
    showNotify({ type: 'danger', message: '添加失败' })
  }

  loading.value = false
}
</script>

<style scoped>
.add-container {
  min-height: 100vh;
  background-color: #f5f5f5;
}

.add-form {
  margin-top: 20px;
}
</style>