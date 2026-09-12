import type { InputHTMLAttributes } from 'react'

type TextInputProps = InputHTMLAttributes<HTMLInputElement>

export default function TextInput({ className = '', ...props }: TextInputProps) {
  return (
    <input
      className={`w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm text-gray-900 placeholder:text-gray-400 transition focus:border-purple-700 focus:outline-none focus:ring-2 focus:ring-purple-700/30 disabled:cursor-not-allowed disabled:bg-gray-100 disabled:text-gray-400 ${className}`}
      {...props}
    />
  )
}
